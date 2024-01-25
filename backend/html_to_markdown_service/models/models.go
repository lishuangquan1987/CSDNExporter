package models

import "github.com/gin-gonic/gin"

type Request struct {
	Url string
}

type Response struct {
	IsSuccess bool
	ErrorMsg  string
	Result    string
}

func OK(ctx *gin.Context, result string) {
	ctx.JSON(200, Response{
		IsSuccess: true,
		Result:    result,
	})
}
func NG(ctx *gin.Context, errMsg string) {
	ctx.JSON(200, Response{
		IsSuccess: false,
		ErrorMsg:  errMsg,
	})
}
func NGWithError(ctx *gin.Context, err error) {
	ctx.JSON(200, Response{
		IsSuccess: false,
		ErrorMsg:  err.Error(),
	})
}
