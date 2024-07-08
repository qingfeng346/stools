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
            addTime: DataTypes.DATE,            //文件添加时间
            title: DataTypes.STRING(128),       //标题
            duration: DataTypes.INTEGER,        //时长
            album: DataTypes.INTEGER,           //专辑
            artist: DataTypes.JSON,             //艺术家
            year: DataTypes.INTEGER,            //年份
            track: DataTypes.INTEGER,           //曲目
            lyrics: DataTypes.TEXT,             //歌词
        }, {
            tableName: "music",
            comment: "音乐列表",
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
            comment: "歌手列表",
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
            comment: "专辑列表",
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
    }
    async sync() {
        await this.music.sync({alter: true})
        await this.artist.sync({alter: true})
        await this.album.sync({alter: true})
    }
}
export default new database()