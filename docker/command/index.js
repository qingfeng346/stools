require('weimingcommons').logger.ilog = require('./src/logger')
async function main() {
    await require('./src/Build').startBuild(process.argv[2])
}
main()