package lights

import (
	"net/http"
)

// Lights is an HTTP Cloud Function with a request parameter.
// gcloud functions deploy Lights --runtime go113 --trigger-http --region us-east4 --memory=128MB
func Lights(w http.ResponseWriter, r *http.Request) {
	Run()
}
