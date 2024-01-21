package models

type Request struct {
	Url string
}

type Response struct {
	IsSuccess bool
	ErrorMsg  string
	Result    string
}
