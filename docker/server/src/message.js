class message {
    constructor() {
        this.events = {}
    }
    register(code, func) {
        this.events[code] = func
    }
    get(code) {
        return this.events[code]
    }
}
module.exports = new message()