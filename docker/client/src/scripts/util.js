import { RadioGroup,Radio,Input } from 'view-ui-plus'
class util {
    init($Message, $Modal, homePage) {
        this.$Message = $Message
        this.$Modal = $Modal
        this.homePage = homePage
        this.filterList = [
            { type: "name", label: "名字" },
            { type: "album", label: "专辑" },
            { type: "singer", label: "歌手" }
        ]
    }
    async getClipboardText() {
        return await navigator.clipboard.readText();
    }
    getFilter(type) {
        return this.filterList.find((item) => { return item.type == type })
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
    confirmFilter(onOk) {
        this.homePage.ShowFilter(onOk)
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
    confirm(info) {
        if (info.closable == undefined) { info.closable = true }
        this.$Modal.confirm(info)
    }
}
export default new util()