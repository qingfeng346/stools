package main

import (
	"flag"
	"fmt"
)

var (
	// 命令行参数
	tFlag = flag.String("t", "默认值", "用户名")
	uFlag = flag.String("u", "默认值", "`faweawef`")
)

func main() {

	// ttt := *tFlag
	// var ttt string
	// var uuu string
	// flag.StringVar(&ttt, "t", "默认值", "用户名")
	// flag.StringVar(&uuu, "u", "默认值", "用户名")
	flag.Parse()
	fmt.Println(*tFlag, *uFlag)
	flag.Usage()
}
