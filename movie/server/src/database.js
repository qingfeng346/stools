const logger = require('log4js').getLogger('database.js')
const { Sequelize, DataTypes } = require("sequelize")

class database {
    async init() {
        let sequelize = new Sequelize({
            dialect: "sqlite",
            storage: "./data/database/database.db",
            logging: msg => logger.info(msg),
            retry: {
                match: [
                  /SQLITE_BUSY/,
                ],
                name: 'query',
                max: 5
            },
            pool: {
                maxactive: 1,
                max: 5,
                min: 0,
                idle: 20000
            },
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
            releaseDate: DataTypes.STRING(128),
            thumbUrl: DataTypes.STRING(256),
            imageUrl: DataTypes.STRING(256),
            actors: DataTypes.JSON,
            tags: {
                type: DataTypes.JSON,
                comment: "标签"
            },
            makers: {
                type: DataTypes.JSON,
                comment: "制造"
            },
            genres: {
                type: DataTypes.JSON,
                comment: "发行"
            },
            series: {
                type: DataTypes.JSON,
                comment: "系列"
            },
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
        // await this.config.upsert({ name: "name1", tags: [1,2,3,4,5], }, { where: { name: "name1" } })
        // await this.config.upsert({ name: "name2", tags: [{name: "aaa"}], }, { where: { name: "name2" } })
        // await this.config.upsert({ name: "name3", tags: {a:'a', b:'b'}, }, { where: { name: "name3" } })

        // // let www = await this.config.findAll()
        // // console.log(www)
        // // let resuts = await sequelize.query("select * from `config` where exists (select 1 from json_each(tags) where value->>'name' = 'aaa')", { type: QueryTypes.SELECT })
        // // console.log(resuts)
        // // resuts = await sequelize.query("select * from `config` where exists (select 1 from json_each(tags) where value = 1)", { type: QueryTypes.SELECT })
        // // console.log(resuts)
        // let resuts = await sequelize.query("select * from `config` where json_extract(tags, '$.a') = '22'", { type: QueryTypes.SELECT })
        // console.log(resuts)
        // const tagToSearch = 1;
        // const users = await this.config.findAll({
        //   where: sequelize.literal(`JSON_each.value = '${tagToSearch}'`),
        //   include: [{
        //     model: this.config, 
        //     where: sequelize.literal(`JSON_each.key = 'value'`)
        //   }]
        // });

        // const records = await this.config.findAll({
        //     where: {
        //         value: { [Op.contains]: 1 }
        //     }
        // });
        // await this.sync()
        // let result = await this.actor.upsert({name:"234"}, { where: { name: "234" } })
        // console.log(result)
    }
    async sync() {
        await this.movie.sync({alter: true})
        await this.actor.sync({alter: true})
    }
}
module.exports = new database()