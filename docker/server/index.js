async function main() {
    await require('./data').init()
    await require('./net').init()
}
main()