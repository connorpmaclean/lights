package lights

import (
	"bytes"
	"encoding/json"
	"fmt"
	"io/ioutil"
	"net/http"
	"time"
)

func UnmarshalSunriseSunset(data []byte) (SunriseSunset, error) {
	var r SunriseSunset
	err := json.Unmarshal(data, &r)
	return r, err
}

func (r *SunriseSunset) Marshal() ([]byte, error) {
	return json.Marshal(r)
}

type SunriseSunset struct {
	Results Results `json:"results"`
	Status  string  `json:"status"`
}

type Results struct {
	Sunrise                   string `json:"sunrise"`
	Sunset                    string `json:"sunset"`
	SolarNoon                 string `json:"solar_noon"`
	DayLength                 string `json:"day_length"`
	CivilTwilightBegin        string `json:"civil_twilight_begin"`
	CivilTwilightEnd          string `json:"civil_twilight_end"`
	NauticalTwilightBegin     string `json:"nautical_twilight_begin"`
	NauticalTwilightEnd       string `json:"nautical_twilight_end"`
	AstronomicalTwilightBegin string `json:"astronomical_twilight_begin"`
	AstronomicalTwilightEnd   string `json:"astronomical_twilight_end"`
}

type Lifx []LifxElement

func UnmarshalLifx(data []byte) (Lifx, error) {
	var r Lifx
	err := json.Unmarshal(data, &r)
	return r, err
}

func (r *Lifx) Marshal() ([]byte, error) {
	return json.Marshal(r)
}

type LifxElement struct {
	ID               string  `json:"id"`
	UUID             string  `json:"uuid"`
	Label            string  `json:"label"`
	Connected        bool    `json:"connected"`
	Power            string  `json:"power"`
	Color            Color   `json:"color"`
	Brightness       float64 `json:"brightness"`
	Effect           string  `json:"effect"`
	Group            Group   `json:"group"`
	Location         Group   `json:"location"`
	Product          Product `json:"product"`
	LastSeen         string  `json:"last_seen"`
	SecondsSinceSeen int64   `json:"seconds_since_seen"`
	Zones            *Zones  `json:"zones,omitempty"`
}

type Color struct {
	Hue        float64 `json:"hue"`
	Saturation int64   `json:"saturation"`
	Kelvin     int64   `json:"kelvin"`
}

type Group struct {
	ID   string `json:"id"`
	Name string `json:"name"`
}

type Product struct {
	Name         string       `json:"name"`
	Identifier   string       `json:"identifier"`
	Company      string       `json:"company"`
	Capabilities Capabilities `json:"capabilities"`
}

type Capabilities struct {
	HasColor             bool  `json:"has_color"`
	HasVariableColorTemp bool  `json:"has_variable_color_temp"`
	HasIR                bool  `json:"has_ir"`
	HasChain             bool  `json:"has_chain"`
	HasMatrix            bool  `json:"has_matrix"`
	HasMultizone         bool  `json:"has_multizone"`
	MinKelvin            int64 `json:"min_kelvin"`
	MaxKelvin            int64 `json:"max_kelvin"`
}

type Zones struct {
	Count int64  `json:"count"`
	Zones []Zone `json:"zones"`
}

type Zone struct {
	Brightness int64 `json:"brightness"`
	Hue        int64 `json:"hue"`
	Kelvin     int64 `json:"kelvin"`
	Saturation int64 `json:"saturation"`
	Zone       int64 `json:"zone"`
}

type LifxChangeRequest struct {
	States LifxChanges `json:"states"`
	Fast   bool        `json:"fast"`
}

type LifxChanges []LifxChange

type LifxChange struct {
	Selector string `json:"selector"`
	Color    string `json:"color"`
}

func (r *LifxChangeRequest) Marshal() ([]byte, error) {
	return json.Marshal(r)
}

const SEATTLE_LAT float64 = 47.60357
const SEATTLE_LON float64 = -122.32945

const kelvinHigh int = 5500
const kelvinLow int = 3000

func getPSTLocation() *time.Location {
	location, _ := time.LoadLocation("America/Los_Angeles")
	return location
}

func parseAndConvertToLocal(utcInput string) time.Time {
	utcTime, timeParseErr := time.Parse("3:04:05 PM", utcInput)
	if timeParseErr != nil {
		fmt.Println()
		fmt.Printf("Parse error: %s", timeParseErr)
	}

	location := getPSTLocation()
	targetTime := utcTime.In(location)

	zeroTime := time.Date(0, 1, 1, 0, 0, 0, 0, location)
	if targetTime.Before(zeroTime) {
		oneDay, _ := time.ParseDuration("24h")
		targetTime = targetTime.Add(oneDay)
	}

	localDate := time.Now().In(location)
	return time.Date(localDate.Year(), localDate.Month(), localDate.Day(), targetTime.Hour(), targetTime.Minute(), targetTime.Second(), targetTime.Nanosecond(), location)
}

func linearMap(value, fromSource, toSource, fromTarget, toTarget float64) float64 {
	return (value-fromSource)/(toSource-fromSource)*(toTarget-fromTarget) + fromTarget
}

// Runs the lights program
func Run() {
	var url string = fmt.Sprintf("https://api.sunrise-sunset.org/json?lat=%f&lng=%f", SEATTLE_LAT, SEATTLE_LON)
	fmt.Println(url)
	resp, err := http.Get(url)
	if err != nil {
		fmt.Println()
		fmt.Printf("Sunrise-Sunset error: %s", err)
	}
	defer resp.Body.Close()
	body, _ := ioutil.ReadAll(resp.Body)

	sunriseSunset, _ := UnmarshalSunriseSunset(body)

	fmt.Printf("Raw Sunrise %s, Sunset %s", sunriseSunset.Results.Sunrise, sunriseSunset.Results.Sunset)
	fmt.Println()

	sunrise := parseAndConvertToLocal(sunriseSunset.Results.Sunrise)
	sunset := parseAndConvertToLocal(sunriseSunset.Results.Sunset)

	minusTwoHours, _ := time.ParseDuration("-2h")

	var sunriseStart, sunriseEnd int64 = sunrise.Add(minusTwoHours).Unix(), sunrise.Unix()
	var sunsetStart, sunsetEnd int64 = sunset.Add(minusTwoHours).Unix(), sunset.Unix()

	fmt.Printf("Local Sunrise %s, Sunset %s", sunrise, sunset)

	now := time.Now().Unix()
	var targetValue int

	switch {
	case now < sunriseStart:
		targetValue = kelvinLow
	case now < sunriseEnd:
		targetValue = int(linearMap(float64(now), float64(sunriseStart), float64(sunriseEnd), float64(kelvinLow), float64(kelvinHigh)))
	case now < sunsetStart:
		targetValue = kelvinHigh
	case now < sunsetEnd:
		targetValue = int(linearMap(float64(now), float64(sunriseStart), float64(sunriseEnd), float64(kelvinHigh), float64(kelvinLow)))
	default:
		targetValue = kelvinLow
	}

	fmt.Println()
	fmt.Printf("Target Value %d", targetValue)

	req, _ := http.NewRequest("GET", "https://api.lifx.com/v1/lights/all", nil)
	client := &http.Client{}
	req.Header.Add("Authorization", fmt.Sprintf("Bearer %s", "c03c804ea9ae1471ab6549d2e3783e26b5dcd999abd6f15ed84fd07fc148c907"))
	resp2, err := client.Do(req)
	if err != nil {
		fmt.Println()
		fmt.Printf("Sunrise-Sunset error: %s", err)
	}

	defer resp2.Body.Close()
	body2, _ := ioutil.ReadAll(resp2.Body)

	lifxLights, _ := UnmarshalLifx(body2)

	var changeList LifxChanges
	for _, element := range lifxLights {
		currentKelvin := element.Color.Kelvin
		fmt.Println()
		if element.Power != "on" {
			fmt.Printf("Skipping %s because they are off", element.Label)
		} else if int(currentKelvin) != targetValue {
			fmt.Printf("Changing %s from %d to %d", element.Label, currentKelvin, targetValue)
			change := LifxChange{Color: fmt.Sprintf("kelvin:%d", targetValue), Selector: fmt.Sprintf("label:%s", element.Label)}
			changeList = append(changeList, change)
		} else {
			fmt.Printf("%s is already at %d", element.Label, targetValue)
		}

	}

	if len(changeList) > 0 {
		changeRequest := LifxChangeRequest{States: changeList, Fast: true}
		json, _ := changeRequest.Marshal()
		request2, _ := http.NewRequest("PUT", "https://api.lifx.com/v1/lights/states", bytes.NewBuffer(json))
		request2.Header.Add("Authorization", fmt.Sprintf("Bearer %s", "c03c804ea9ae1471ab6549d2e3783e26b5dcd999abd6f15ed84fd07fc148c907"))
		resp3, err := client.Do(request2)
		fmt.Println()
		if err != nil {
			fmt.Printf("Lifx API error: %s", err)
		} else {
			fmt.Printf("Submitted change: %s", json, resp3)
		}
	}

}
