import log4js from 'log4js'
import { Sequelize, DataTypes } from "sequelize"
const logger = log4js.getLogger('database.js')
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
        this.music = sequelize.define('music', {
            id: {
                type: DataTypes.INTEGER,
                primaryKey: true,
                autoIncrement: true,
            },
            path: DataTypes.STRING(128),        //路径
            size: DataTypes.INTEGER,            //文件大小
            ctime: DataTypes.DATE,              //文件最后修改时间
            version: DataTypes.INTEGER,         //文件版本号
            title: DataTypes.STRING(128),       //标题
            album: DataTypes.INTEGER,           //专辑
            artist: DataTypes.JSON,             //艺术家
            year: DataTypes.INTEGER,            //年份
            track: DataTypes.INTEGER,           //曲目
            lyrics: DataTypes.TEXT,             //歌词
        }, {
            tableName: "music",
            timestamps: false,
            indexes: [
                {
                    unique: true,
                    fields: ['path']
                }
            ],
        });
        await this.music.sync()
        this.artist = sequelize.define("artist", {
            id: {
                type: DataTypes.INTEGER,
                primaryKey: true,
                autoIncrement: true,
            },
            name: DataTypes.STRING(128),
        }, {
            tableName: "artist",
            timestamps: false,
            indexes: [
                {
                    unique: true,
                    fields: ['name']
                }
            ],
        })
        await this.artist.sync()
        this.album = sequelize.define("album", {
            id: {
                type: DataTypes.INTEGER,
                primaryKey: true,
                autoIncrement: true,
            },
            name: DataTypes.STRING(256),
            artist: DataTypes.JSON,
        }, {
            tableName: "album",
            timestamps: false,
            indexes: [
                {
                    unique: true,
                    fields: ['name']
                }
            ],
        })
        await this.album.sync()
        await this.sync()
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
        await this.music.sync({alter: true})
        await this.artist.sync({alter: true})
        await this.album.sync({alter: true})
    }
}
export default new database()