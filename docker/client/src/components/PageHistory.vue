<template>
  <Layout>
    <Content>
      <Table
        :columns="columns"
        :height="tableHeight"
        :row-class-name="GetClassName"
        :data="datas"
        :loading="loading"
      />
      <div class="bottom">
        <Input v-model="searchValue" class="search" search @on-search="OnClickSearch" />
        <Page
          class="page"
          v-if="!loading"
          :current="page"
          :total="pageTotal"
          :page-size="pageSize"
          show-total
          show-elevator
          @on-change="OnChangePage"
        />
      </div>
    </Content>
  </Layout>
</template>
<script>
import RowHistory from "./row/RowHistory";
import net from "../scripts/net";
import util from "../scripts/util";
import { Status, MsgCode } from "../scripts/code";
export default {
  data() {
    return {
      page: 1, //当前页数
      pageTotal: 1, //总页数
      pageSize: 1, //每一页显示条数,服务器决定
      loading: false, //是否正在加载
      tableHeight: 1, //窗口高度
      searchValue: "", //搜索关键字
      datas: [], //所有数据
    };
  },
  beforeMount() {
    this.columns = [
      {
        type: "expand",
        width: 30,
        render: (h, params) => {
          return h(RowHistory, {
            props: {
              formData: params.row.infos,
              history: params.row,
            },
          });
        },
      },
      {
        title: "ID",
        key: "id",
        width: 160,
        render: (h, params) => {
          return h(
            "router-link",
            {
              attrs: {
                to: `/home/historyinfo?id=${params.row.id}`,
              },
              style: {
                color: "white",
                "text-decoration": "underline",
              },
            },
            params.row.id
          );
        },
      },
      { title: "平台", key: "platform", width: 90 },
      { title: "分支", key: "branch", width: 100 },
      { title: "环境", key: "environment", width: 150 },
      { 
        title: "服务器", 
        key: "serverLabel",
        width: 200,
        render: (h, params) => {
          return h(
            "a", {
              attrs: {
                href: params.row.serverUrl,
                target: "_blank"
              },
              style: {
                color: "white",
                "text-decoration": "underline",
              },
            },
            params.row.serverLabel
          );
        },
      },
      { title: "类型", key: "operate", minWidth: 140 },
      { 
        title: "提交用户",
        key: "address",
        minWidth: 120,
        render: (h, params) => {
          var users = params.row.address.split(",");
          var datas = [];
          for (var i = 0; i < users.length; i++) {
            if (i > 0)
              datas.push(h("br"));
              datas.push(h("span", users[i]));
          }
          return h("span", datas);
        }
      },
    ];
    if (util.HasAuth("0202")) {
      this.columns.push({
        title: "操作",
        key: "operateRow",
        width: 100,
        render: (h, params) => {
          return h("div", [
            h(
              "Button",
              {
                props: { type: "error" },
                on: {
                  click: () => {
                    this.OnClickDeleteHistory(params.row);
                  },
                },
              },
              "删除"
            ),
          ]);
        },
      });
    }
  },
  mounted() {
    util.registerOnResize(this, [{ name: "tableHeight", value: 70 }]);
    this.RefreshTable();
    net.registerMessage(MsgCode.refreshHistorys, () => {
      this.RefreshTable();
    });
  },
  methods: {
    //刷新所有信息
    async RefreshTable() {
      this.loading = true;
      let data = await util.requestHistorys(
        this.page,
        this.searchValue,
        this.filters
      );
      this.loading = false;
      if (!data) {
        return;
      }
      this.pageTotal = data.pageTotal;
      this.pageSize = data.pageSize;
      this.datas = [];
      for (let history of data.datas) {
        this.datas.push(await util.parseHistory(history));
      }
    },
    //每一行的背景显示
    GetClassName(row, index) {
      if (row.status == Status.success) {
        return "table-row-success";
      } else if (row.status == Status.fail) {
        return "table-row-error";
      } else if (row.status == Status.process) {
        return "table-row-info";
      } else if (row.status == Status.wait) {
        return "table-row-wait";
      }
      return "table-row-error";
    },
    //删除一条历史记录
    OnClickDeleteHistory(row) {
      util.confirm({
        title: "警告",
        content: `<p>确定要删除记录 ${row.id} 吗?<br>此操作会删除项目工程(Xcode 或 AndroidStudio), 但是不会删除 apk & ipa 文件.</p>`,
        okText: "删除",
        cancelText: "取消",
        onOk: async () => {
          await util.deleteHistory(row.id);
          this.RefreshTable();
        },
      });
    },
    //点击页数
    OnChangePage(page) {
      this.page = page;
      this.RefreshTable();
    },
    //点击搜索
    OnClickSearch() {
      this.RefreshTable();
    },
  },
};
</script>
<style>
.search {
  width: 300px;
}
.ivu-table .table-row-wait td {
  background-color: #c2c2c2;
  color: rgb(255, 255, 255);
}
.ivu-table .table-row-info td {
  background-color: #26b5f7;
  color: rgb(255, 255, 255);
}
.ivu-table .table-row-success td {
  background-color: #01da01;
  color: #fff;
}
.ivu-table .table-row-release td {
  background-color: #eeb600;
  color: #fff;
}
.ivu-table .table-row-error td {
  background-color: hsl(0, 100%, 50%);
  color: #fff;
}
</style>
