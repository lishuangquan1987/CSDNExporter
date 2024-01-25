package main

import (
	"github.com/gin-gonic/gin"
	"html_to_markdown_service/routers"
)

func main() {
	r := gin.Default()
	routers.InitRouters(r)
	panic(r.Run(":8081"))
}
