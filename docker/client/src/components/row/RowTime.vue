<template>
  <span>{{ date }}</span>
</template>
<script>
import { Util } from "weimingcommons";
export default {
  props: {
    time: {
      type: [Number, Date, String],
      required: true,
    },
    interval: {
      type: Number,
      default: 1,
    },
  },
  data() {
    return {
      date: "",
    };
  },
  methods: {
    setTime() {
      let startTime = Util.formatDate(this.time);
      this.date = `${startTime}  至今, 已耗时 ${Util.getElapsedTimeString(this.time)}`;
    },
  },
  mounted() {
    this.setTime();
    this.timer = setInterval(() => {
      this.setTime();
    }, 1000 * this.interval);
  },
  beforeDestroy() {
    if (this.timer) clearInterval(this.timer);
  },
};
</script>
