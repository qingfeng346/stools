import { RadioGroup,Radio,Input } from 'view-ui-plus'
class util {
    init($Message, $Modal, homePage) {
        this.$Message = $Message
        this.$Modal = $Modal
        this.homePage = homePage
    }
    async getClipboardText() {
        return await navigator.clipboard.readText();
    }
    noticeInfo(msg) {
        this.$Message.info({
            duration: 6,
            render: h => {
                return h('pre', [msg])
            }
        })
    }
    noticeSuccess(msg) {
        this.$Message.success({
            duration: 6,
            render: h => {
                return h('pre', [msg])
            }
        })
    }
    noticeError(msg) {
        this.$Message.error({
            duration: 6,
            render: h => {
                return h('pre', [msg])
            }
        })
    }
    log(msg) {
        console.log(msg)
        this.homePage.AddLog(msg)
    }
    confirmBox(title, content, onOk, closeable) {
        let config = {
            title: title,
            content: content,
            okText: "确定",
            cancelText: "关闭",
            closeable: closeable ?? true,
        }
        if (onOk != null) {
            config.onOk = onOk
        }
        this.confirm(config)
    }
    confirmFilter(onOk) {
        let text = ""
        let config = {
            okText: "确定",
            cancelText: "关闭",
            closeable: false,
            render: (h) => {
                return h("div", [
                    // h(RadioGroup, {
                    //     onChange: (v,v1) => {
                    //         // console.log(v, v1)
                    //     },
                    // },
                    // [
                    //     h(Radio, {
                    //         label: "name",
                    //         onChange: (v,v1) => {
                    //             console.log(v, v1)
                    //         },
                    //     },() => "名字"),
                    //     h(Radio, {
                    //         label: "album",
                    //         onChange: (v,v1) => {
                    //             console.log(v, v1)
                    //         },
                    //     },() => "专辑")
                    // ],
                    // ),
                    h(Input, {
                        "onInput": (v) => {
                            text = v.target.value
                        }
                    })
                ])
            }
        }
        if (onOk != null) {
            config.onOk = () => {
                onOk?.(text)
            }
        }
        this.confirm(config)
    }
    confirm(info) {
        if (info.closable == undefined) { info.closable = true }
        this.$Modal.confirm(info)
    }
}
export default new util()