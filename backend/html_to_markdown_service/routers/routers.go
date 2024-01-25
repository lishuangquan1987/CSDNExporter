package routers

import (
	"github.com/gin-gonic/gin"
	"html_to_markdown_service/controllers"
)

func InitRouters(r *gin.Engine) {
	r.POST("/get_markdown_string", controllers.GetMarkDownString)
}
