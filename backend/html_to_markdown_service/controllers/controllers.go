package controllers

import (
	md "github.com/JohannesKaufmann/html-to-markdown"
	"github.com/gin-gonic/gin"
	"html_to_markdown_service/models"
	"io"
	"net/http"
)

var converter = md.NewConverter("", true, nil)

func GetMarkDownString(ctx *gin.Context) {
	req := models.Request{}
	if err := ctx.ShouldBind(&req); err != nil {
		models.NGWithError(ctx, err)
		return
	}

	if resp, err := http.Get(req.Url); err != nil {
		models.NGWithError(ctx, err)
	} else {
		var bodyStr string
		if body, err := io.ReadAll(resp.Body); err != nil {
			models.NGWithError(ctx, err)
			return
		} else {
			bodyStr = string(body)
		}

		if markdown, err := converter.ConvertString(bodyStr); err != nil {
			models.NGWithError(ctx, err)
		} else {
			models.OK(ctx, markdown)
		}
	}
}
