<template>
  <span>{{ date }}</span>
</template>
<script>
import { ref } from "vue"
import { Util } from "weimingcommons";
export default {
  setup(props) {
    const date = ref("")
    return {
      date,
    }
  },
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
