const logger = require('log4js').getLogger('database.js')
const { Sequelize, DataTypes } = require("sequelize")

class database {
    async init() {
        let sequelize = new Sequelize({
            dialect: "sqlite",
            storage: "./data/database/database.sqlite",
            logging: msg => logger.info(msg)
        })
        this.sequelize = sequelize
        this.config = sequelize.define('config', {
            key: {
                type: DataTypes.STRING(64),
                primaryKey: true
            },
            value: DataTypes.TEXT,
        }, {
            tableName: "config",
            timestamps: false,
        });
        await this.config.sync()
        this.history = sequelize.define("history", {
            id: {
                type: DataTypes.CHAR(32),
                primaryKey: true,
                autoIncrement: false,
                comment: "执行ID",
            },
            createTime: {
                type: DataTypes.BIGINT,
                comment: "创建时间"
            },
            startTime: {
                type: DataTypes.BIGINT,
                comment: "开始执行时间",
            },
            endTime: {
                type: DataTypes.BIGINT,
                comment: "执行结束时间"
            },
            username: {
                type: DataTypes.CHAR(64),
                comment: "创建命令的用户",
            },
            serverAddress: {
                type: DataTypes.CHAR(128),
                comment: "服务器地址",
            },
            operate: {
                type: DataTypes.CHAR(64),
                comment: "操作类型",
            },
            command: {
                type: DataTypes.STRING(2048),
                comment: "命令"
            },
            status: {
                type: DataTypes.CHAR(32),
                comment: "当前状态"
            },
            result: {
                type: DataTypes.TEXT,
                comment: "返回结果",
            },
            urls: {
                type: DataTypes.STRING(2048),
                allowNull: false,
                defaultValue: "",
                comment: "生成的urls",
            }
        }, {
            tableName: "history",
            timestamps: false,
        })
        await this.history.sync()
        this.command = sequelize.define("command", {
            name: {
                type: DataTypes.CHAR(64),
                primaryKey: true,
                comment: "Name",
            },
            info: DataTypes.TEXT,       //基础信息
            content: DataTypes.TEXT,    //命令参数
            execute: DataTypes.TEXT,    //执行命令
            operate: DataTypes.TEXT,    //二次操作
        }, {
            tableName: "command",
            timestamps: false,
        })
        await this.command.sync()
    }
}
module.exports = new database()