package main

import (
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

const SEATTLE_LAT float64 = 47.60357
const SEATTLE_LON float64 = -122.32945

func parseAndConvertToLocal(utcInput string) time.Time {
	utcTime, timeParseErr := time.Parse("3:04:05 PM", utcInput)
	if timeParseErr != nil {
		fmt.Println()
		fmt.Printf("Parse error: %s", timeParseErr)
	}

	location, _ := time.LoadLocation("America/Los_Angeles")
	return utcTime.In(location)
}

// Runs the lights program
func Run() {
	var url string = fmt.Sprintf("https://api.sunrise-sunset.org/json?lat=%f&lng=%f", SEATTLE_LAT, SEATTLE_LON)
	fmt.Println(url)
	resp, err := http.Get(url)
	if err != nil {
		// handle error
	}
	defer resp.Body.Close()
	body, err := ioutil.ReadAll(resp.Body)

	sunriseSunset, jsonErr := UnmarshalSunriseSunset(body)
	if jsonErr != nil {
		// handle error
	}

	fmt.Printf("Raw Sunrise %s, Sunset %s", sunriseSunset.Results.Sunrise, sunriseSunset.Results.Sunset)
	fmt.Println()

	sunrise := parseAndConvertToLocal(sunriseSunset.Results.Sunrise)
	sunset := parseAndConvertToLocal(sunriseSunset.Results.Sunset)

	fmt.Printf("Local Sunrise %s, Sunset %s", sunrise, sunset)

}
