<style scoped>
.layout{
    border: 1px solid #d7dde4;
    background: #f5f7f9;
}
.layout-logo{
    width: 100px;
    height: 30px;
    background: #5b6270;
    border-radius: 3px;
    float: left;
    position: relative;
    top: 15px;
    left: 20px;
}
.layout-nav{
    width: 420px;
    margin: 0 auto;
}
.layout-assistant{
    width: 300px;
    margin: 0 auto;
    height: inherit;
}
.layout-breadcrumb{
    padding: 10px 15px 0;
}
.layout-content{
    min-height: 200px;
    margin: 15px;
    overflow: hidden;
    background: #fff;
    border-radius: 4px;
}
.layout-content-main{
    padding: 10px;
}
.layout-copy{
    text-align: center;
    padding: 10px 0 20px;
    color: #9ea7b4;
}
</style>
<template>
    <div class="layout">
        <Menu mode="horizontal" theme="dark" active-name="1">
            <div class="layout-logo"></div>
            <div class="layout-nav">
                <MenuItem name="1">
                    <Icon type="ios-navigate"></Icon>
                    Item 1
                </MenuItem>
                <MenuItem name="2">
                    <Icon type="ios-keypad"></Icon>
                    Item 2
                </MenuItem>
                <MenuItem name="3">
                    <Icon type="ios-analytics"></Icon>
                    Item 3
                </MenuItem>
                <MenuItem name="4">
                    <Icon type="ios-paper"></Icon>
                    Item 4
                </MenuItem>
            </div>
        </Menu>
        <Menu mode="horizontal" active-name="1">
            <div class="layout-assistant">
                <MenuItem name="1">Option 1</MenuItem>
                <MenuItem name="2">Option 2</MenuItem>
                <MenuItem name="3">Option 3</MenuItem>
            </div>
        </Menu>
        <div class="layout-breadcrumb">
            <Breadcrumb>
                <BreadcrumbItem href="#">Home</BreadcrumbItem>
                <BreadcrumbItem href="#">Projects</BreadcrumbItem>
                <BreadcrumbItem>iView</BreadcrumbItem>
            </Breadcrumb>
        </div>
        <div class="layout-content">
            <div class="layout-content-main">Content</div>
        </div>
        <div class="layout-copy">
            2011-2016 &copy; TalkingData
        </div>
    </div>
</template>
<script>
    export default {
        
    }
</script>