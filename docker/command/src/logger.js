const { Util } = require('weimingcommons')
class logger {
    get NowDate() { 
        return Util.formatDate(new Date())
    }
    write(str) {
        process.stdout.write(str)
    }
    writeError(str) {
        process.stderr.write(str)
    }
    info(str) {
        this.log(str)
    }
    log(str) {
        str = `[${this.NowDate}] [info] ${str}`
        console.log(str)
    }
    warn(str) {
        str = `[${this.NowDate}] [warn] ${str}`
        console.info(str);
    }
    error(str) {
        str = `[${this.NowDate}] [error] ${str}`
        console.info(str);
    }
}
module.exports = new logger();