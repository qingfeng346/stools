<template>
  <span>{{ date }}</span>
</template>
<script>
import { ref } from "vue"
import { Util } from "weimingcommons";
export default {
  props: {
    time: {
      type: Number,
      required: true,
    },
    interval: {
      type: Number,
      default: 1,
    },
  },
  setup() {
    const date = ref("")
    return {
      date,
    }
  },
  mounted() {
    this.setTime();
    this.timer = setInterval(() => {
      this.setTime();
    }, 1000 * this.interval);
  },
  beforeUnmount() {
    if (this.timer) clearInterval(this.timer);
  },
  methods: {
    setTime() {
      this.date = `${Util.formatDate(new Date(this.time))} 至今, 已耗时 ${Util.getElapsedTime(this.time)}`;
    },
  },
};
</script>
