const logger = require('log4js').getLogger('database.js')
const { Sequelize, DataTypes } = require("sequelize")

class database {
    async init() {
        let sequelize = new Sequelize({
            dialect: "sqlite",
            storage: "./data/database/database.db",
            logging: msg => logger.info(msg)
        })
        this.sequelize = sequelize
        this.movie = sequelize.define('movie', {
            id: {
                type: DataTypes.INTEGER,
                primaryKey: true,
                autoIncrement: true,
            },
            path: DataTypes.STRING(128),
            title: DataTypes.TEXT,
            desc: DataTypes.TEXT,
            releaseDate: DataTypes.DATE,
            thumbUrl: DataTypes.STRING(256),
            imageUrl: DataTypes.STRING(256),
            actors: DataTypes.JSON,
            tags: DataTypes.JSON,
            shotscreens: DataTypes.JSON,
            isInfo: DataTypes.BOOLEAN,
        }, {
            tableName: "movie",
            timestamps: false,
            indexes: [
                {
                    unique: true,
                    fields: ['path']
                }
            ],
        });
        await this.movie.sync()
        this.actor = sequelize.define("actor", {
            id: {
                type: DataTypes.INTEGER,
                primaryKey: true,
                autoIncrement: true,
            },
            name: DataTypes.STRING(128),
            desc: DataTypes.TEXT,
            beginDate: DataTypes.DATE,
            endDate: DataTypes.DATE,
            imageUrl: DataTypes.STRING(256),
            isInfo: DataTypes.BOOLEAN,
        }, {
            tableName: "actor",
            timestamps: false,
            indexes: [
                {
                    unique: true,
                    fields: ['name']
                }
            ],
        })
        await this.actor.sync()
        this.tag = sequelize.define("tag", {
            id: {
                type: DataTypes.INTEGER,
                primaryKey: true,
                autoIncrement: true,
            },
            name: DataTypes.STRING(64)
        }, {
            tableName: "tag",
            timestamps: false,
            indexes: [
                {
                    unique: true,
                    fields: ['name']
                }
            ],
        })
        await this.tag.sync()
        await this.sync()
        // let result = (await this.actor.findOrCreate({ where: { name: "123" }}))[0].dataValues;
        // console.log(result)
    }
    async sync() {
        await this.movie.sync({alter: true})
        await this.actor.sync({alter: true})
        await this.tag.sync({alter: true})
    }
}
module.exports = new database()