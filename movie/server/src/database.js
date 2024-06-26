const logger = require('log4js').getLogger('database.js')
const { Sequelize, DataTypes, QueryTypes } = require("sequelize")

class database {
    async init() {


        let sequelize = new Sequelize({
            dialect: "sqlite",
            storage: "./data/database/database.sqlite",
            logging: msg => logger.info(msg)
        })
        this.sequelize = sequelize
        // sequelize.queryInterface.jsonContains = function (field, value) {
        //     return sequelize.where(
        //         sequelize.literal(`json_extract(json_each.value, '$.${field}')`),
        //         value
        //     );
        // };
        this.config = sequelize.define('config', {
            name: {
                type: DataTypes.STRING(64),
                primaryKey: true,
                comment: "Name",
            },
            tags: DataTypes.JSON,
        }, {
            tableName: "config",
            timestamps: false,
        });
        await this.config.sync()
        // await this.config.upsert({ name: "name1", tags: [1,2,3,4,5], }, { where: { name: "name1" } })
        // await this.config.upsert({ name: "name2", tags: [{name: "aaa"}], }, { where: { name: "name2" } })
        await this.config.upsert({ name: "name3", tags: {a:'a', b:'b'}, }, { where: { name: "name3" } })

        // let www = await this.config.findAll()
        // console.log(www)
        // let resuts = await sequelize.query("select * from `config` where exists (select 1 from json_each(tags) where value->>'name' = 'aaa')", { type: QueryTypes.SELECT })
        // console.log(resuts)
        // resuts = await sequelize.query("select * from `config` where exists (select 1 from json_each(tags) where value = 1)", { type: QueryTypes.SELECT })
        // console.log(resuts)
        let resuts = await sequelize.query("select * from `config` where json_extract(tags, '$.a') = '22'", { type: QueryTypes.SELECT })
        console.log(resuts)
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


    }
    async sync() {
        await this.config.sync({alter: true})
    }
}
module.exports = new database()