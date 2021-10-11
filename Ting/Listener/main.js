import Vue from 'vue'
import App from './App'

Vue.config.productionTip = false

//公共函数定义
import * as util from './common/util'
Object.keys(util).forEach(key => {
	Vue.prototype[key] = util[key];
});


//http://47.99.211.28:8233
uni.addInterceptor('request', {
	invoke(args) {
		if (args.url.indexOf('http') == -1) {
			// request 触发前拼接 url 
			uni.showLoading({
				title: '加载中..'
			});
			args.url = 'http://47.99.211.28:8233' + args.url
			console.log('请求地址', args.url);
		}
	},
	success(args) {
		args.data.status = true
	},
	fail(err) {
		uni.showToast({
			title: '请求失败,' + err.errMsg,
			icon: "none",
			duration: 3000
		})
		console.log('interceptor-fail', err)
	},
	complete(res) {
		uni.hideLoading();
		//console.log('interceptor-complete', res)
	}
})



App.mpType = 'app'

const app = new Vue({
	...App
})
app.$mount()
