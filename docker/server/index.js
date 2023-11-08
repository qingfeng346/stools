async function main() {
    await require('./src/database').init()
    await require('./src/net').init()
    await require('./src/MusicUtil').init()
}
main()