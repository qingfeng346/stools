const { Sequelize, DataTypes } = require("sequelize")

class data {
    async init() {
        let sequelize = new Sequelize({
            dialect: "sqlite",
            storage: "./data/data.sqlite"
        })
        this.sequelize = sequelize
        this.music = sequelize.define("music", {
            musicId: {
                type: DataTypes.CHAR(32),
                primaryKey: true,
                autoIncrement: false,
                comment: "音乐ID",
            },
            musicType: {
                type: DataTypes.CHAR(32),
                primaryKey: true,
                autoIncrement: false,
                comment: "音乐平台",
            },
            time: {
                type: DataTypes.BIGINT,
                comment: "下载时间"
            },
            name: {
                type: DataTypes.STRING(64),
                comment: "音乐名字",
            },
            album: {
                type: DataTypes.STRING(64),
                comment: "专辑",
            },
            singer: {
                type: DataTypes.STRING(64),
                comment: "作者",
            },
            year: {
                type: DataTypes.INTEGER,
                comment: "发行年份",
            },
        }, {
            tableName: "music",
            timestamps: false,
        })
        await this.music.sync({ alter: true })
    }
}
module.exports = new data()