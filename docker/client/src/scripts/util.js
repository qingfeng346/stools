import net from './net'
import { RequestCode } from './code'
class util {
    init($Message, $Modal, homePage) {
        this.$Message = $Message
        this.$Modal = $Modal
        this.homePage = homePage
        this.configs = {}
    }
    noticeInfo(msg) {
        this.log(`[Info]${msg}`)
        this.$Message.info({
            duration: 6,
            render: h => {
                return h('pre', [msg])
            }
        })
    }
    noticeSuccess(msg) {
        this.log(`[Success]${msg}`)
        this.$Message.success({
            duration: 6,
            render: h => {
                return h('pre', [msg])
            }
        })
    }
    noticeError(msg) {
        this.log(`[Error]${msg}`)
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
    confirm(info) {
        if (info.closable == undefined) { info.closable = true }
        this.$Modal.confirm(info)
    }
    //获取config缓存
    async GetConfig(key) {
        if (this.configs.hasOwnProperty(key)) {
            return this.configs[key]
        } else {
            return this.configs[key] = (await net.request(RequestCode.GetConfig, { key: key }))
        }
    }
    
}
export default new util()