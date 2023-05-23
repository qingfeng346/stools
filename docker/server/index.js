async function main() {
    await require('./database').init()
    await require('./net').init()
    await require('./music').init()
}
main()