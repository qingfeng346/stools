<template>
  <Layout>
    <Content>
      <Table
        :columns="columns"
        :height="tableHeight"
        :row-class-name="GetClassName"
        :data="datas"
        :loading="loading">
        <template #id="{row}">
          <router-link :to="GetHistoryInfoLink(row)" style="color: white; text-decoration: underline">{{ row.id }}</router-link>
        </template>
      </Table>
      <div class="bottom">
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
import RowHistory from "./row/RowHistory.vue";
import net from "../scripts/net";
import util from "../scripts/util";
import code from '../scripts/code.js';
const { RequestCode, Status } = code;
import { resolveComponent } from 'vue';

export default {
  data() {
    return {
      loading: false, //是否正在加载
      columns: [],
      datas: [], //所有数据
      page: 1, //当前页数
      pageTotal: 1, //总页数
      pageSize: 1, //每一页显示条数,服务器决定
      tableHeight: 1, //窗口高度
    };
  },
  beforeMount() {
    this.columns = [
      {
        type: "expand",
        width: 30,
        render: (h, params) => {
          return h(RowHistory, {
            history: params.row,
          });
        },
      },
      { title: "ID", slot: "id", width: 160 },
      { title: "操作类型", key: "name", minWidth: 140 },
      {
        title: "操作",
        width: 100,
        render: (h, params) => {
          return h("div", [
            h(resolveComponent("Button"), {
                type: "error",
                onClick: () => {
                  this.OnClickDeleteHistory(params.row)
                }
              },
              () => "删除"
            ),
          ]);
        },
      }
    ];
  },
  mounted() {
    util.registerOnResize(this, [{ name: "tableHeight", value: 0 }])
    this.RefreshTable()
  },
  methods: {
    //刷新所有信息
    async RefreshTable() {
      this.loading = true;
      let result = await util.GetHistorys(this.page)
      this.loading = false;
      if (result == null) { return; }
      this.pageTotal = result.pageTotal;
      this.pageSize = result.pageSize;
      this.datas = [];
      for (let history of result.datas) {
        this.datas.push(await util.parseHistory(history));
      }
    },
    GetHistoryInfoLink(row) {
      return ""
      // return `/client/historyinfo?id=${row.id}`
    },
    //每一行的背景显示
    GetClassName(row, index) {
      if (row.status == Status.Success) {
        return "table-row-success";
      } else if (row.status == Status.Fail) {
        return "table-row-error";
      } else if (row.status == Status.Process) {
        return "table-row-info";
      } else if (row.status == Status.Wait) {
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
          await util.DelHistory(row.id)
          this.RefreshTable();
        },
      });
    },
    //点击页数
    OnChangePage(page) {
      this.page = page;
      this.RefreshTable();
    },
  },
};
</script>
<style>
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
