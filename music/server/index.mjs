import log4js from 'log4js'
import axios from 'axios'
import RequestManager from './src/Request/RequestManager.js'
import ServerConfig from './src/Request/ServerConfig.js'
import database from './src/database.js'
import net from './src/net.js'
log4js.configure({
    appenders: {
        console: {
            type: 'console',
            layout: {
                type: "pattern",
                pattern: "[%d{yyyy-MM-dd hh:mm:ss}] [%p] %c:%l - %m"
            }
        },
        file: {
            type: 'dateFile',
            encoding: "utf-8",
            filename: './data/logs/log',
            alwaysIncludePattern: true,
            pattern: "yyyy-MM-dd.log",
            layout: {
                type: "pattern",
                pattern: "[%d{yyyy-MM-dd hh:mm:ss}][%p] %c:%l - %m"
            }
        }
    },
    categories: {
        default: {
            enableCallStack: true,
            appenders: ['console', 'file'], level: 'ALL'
        }
    }
})
const logger = log4js.getLogger('index.js')
logger.info("=========================开始启动=========================")
process.on('exit', (code) => {
    logger.info(`=========================进程关闭:${code}=========================`)
});
async function main() {
    try {
        //监听未捕获的异常
        process.on('uncaughtException', function(err, origin) {
            logger.error(`未捕获的异常 ${err}\n${origin}`)
        })
        //监听Promise没有被捕获的失败函数
        process.on('unhandledRejection', function(err, promise){
            logger.error(`未捕获的promise异常 ${err}`)
        })
        axios.defaults.timeout = 6000
        await database.init()
        await net.init()
        await RequestManager.init()
        await ServerConfig.init()
    } catch (e) {
        logger.error("启动失败 : ", e)
    }
}

main()