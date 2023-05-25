const { Sequelize, DataTypes } = require("sequelize")

class database {
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
                comment: "音乐ID",
            },
            musicType: {
                type: DataTypes.CHAR(32),
                primaryKey: true,
                comment: "音乐平台",
            },
            time: {
                type: DataTypes.BIGINT,
                comment: "下载时间"
            },
            name: {
                type: DataTypes.STRING(128),
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
            size: {
                type: DataTypes.INTEGER,
                comment: "文件大小",
            },
            path: {
                type: DataTypes.STRING(256),
                comment: "文件路径",
            }
        }, {
            tableName: "music",
            timestamps: false,
        })
        await this.music.sync()
    }
}
module.exports = new database()