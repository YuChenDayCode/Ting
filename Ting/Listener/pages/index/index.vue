<template>
	<view class="content">
		<view>
			<uni-search-bar radius="5" placeholder="搜索" clearButton="none" cancelButton="none" @confirm="search" />
		</view>
		<view v-for="(book,index) in Ranking" :key="index">
			<view class="uni-flex uni-row" style="border-bottom: 1px #EEEEEE solid;"
				@click="navigateTo('/pages/book/catalog?id='+book.link+'&name='+book.bookname)">
				<view class="text uni-flex"
					style="color:  #333333; width: 180rpx;height: 200rpx;-webkit-justify-content: center;justify-content: center;-webkit-align-items: center;align-items: center; font-size: 500%;font-weight: 600;">
					{{book.bookname[0]}}
				</view>
				<view class="uni-flex uni-column"
					style="-webkit-flex: 1;flex: 1;-webkit-justify-content: space-between;justify-content: space-between;">
					<view class="text" style="color: black; text-align: left;font-size: 40rpx;">
						{{book.bookname}}
					</view>
					<view class="text"
						style="text-indent: 1.5em; line-height: 32rpx;text-align: left;padding-left: 20rpx;">
						{{book.desc}}
					</view>
					<view class="uni-flex uni-row">
						<view class="text" style="-webkit-flex: 1;flex: 1;">{{book.anthor}}</view>
						<view class="text" style="-webkit-flex: 1;flex: 1;overflow: hidden;">
							{{book.announcer.slice(0,9)}}
						</view>
					</view>
				</view>
			</view>
		</view>
	</view>
</template>

<script>
	export default {
		data() {
			return {
				title: 'Hello',
				Ranking: [{
					bookname: "最强弃少",
					"link": "/mp3/2996.html",
					"anthor": "分类：都市言情",
					"announcer": "播音：克蕾雅",
					"desc": "叶默蓦然清醒过来的时候，才发现周围的一切似乎都变了，美女师....."
				}, {
					"bookname": "流氓艳遇记",
					"link": "/mp3/2562.html",
					"anthor": "分类：都市言情",
					"announcer": "播音：刺儿",
					"desc": "杨洛说国家和国家之间靠的是大炮，男人和男人之间靠的是钞票，....."
				}, {
					"bookname": "凡人修仙传",
					"link": "/mp3/882.html",
					"anthor": "分类：玄幻武侠",
					"announcer": "播音：大灰狼",
					"desc": "讲述了一个普通的山村穷小子，偶然之下，跨入到一个江湖小门派....."
				}, {
					"bookname": "斗破苍穹",
					"link": "/mp3/1433.html",
					"anthor": "分类：玄幻武侠",
					"announcer": "播音：蜡笔小勇 莜儿",
					"desc": "这是一个属于斗气的世界，没有花俏艳丽的魔法，有的，仅仅是繁....."
				}, {
					"bookname": "仙逆",
					"link": "/mp3/1520.html",
					"anthor": "分类：玄幻武侠",
					"announcer": "播音：朱宇",
					"desc": "顺为凡，逆则仙，只在心中一念间\r\n仙逆讲述一个平庸少年，几....."
				}, {
					"bookname": "特种兵在都市",
					"link": "/mp3/4094.html",
					"anthor": "分类：都市言情",
					"announcer": "播音：刺儿",
					"desc": "特种兵在都市(史上最震撼人心的单兵作战能力，全方位解密亦正....."
				}, {
					"bookname": "盘龙",
					"link": "/mp3/3.html",
					"anthor": "分类：玄幻武侠",
					"announcer": "播音：原野",
					"desc": "这本书，讲述了一个拥有盘龙戒指的少年的梦幻旅程。\r\n小山一....."
				}]
			}
		},
		onLoad() {
			this.GetRank();
		},
		methods: {
			search(v) {
				let _this = this;
				console.log(v.value);
				uni.request({
					url: "/api/Ting/GetSearchInfo",
					data: {
						bookname: v.value
					},
					complete: (res) => {
						if (res.data.status) {
							uni.hideLoading();
							_this.Ranking = res.data;
						}
					}
				});
			},
			GetRank() {
				let _this = this;
				uni.request({
					url: "/api/Ting/GetRanking",
					success: (res) => {
						if (res.data.status) {
							_this.Ranking = res.data;
						}
					}
				});
			},
			Getinfo(link) {
				console.log(link);
			},
			navigateTo(url) {
				uni.navigateTo({
					url: url
				});
			}

		}
	}
</script>

<style>
	/* 	.content {
		display: flex;
		flex-direction: column;
		align-items: center;
		justify-content: center;
	} */


	.text-area {
		display: flex;
		justify-content: center;
	}

	.title {
		font-size: 36rpx;
		color: #8f8f94;
	}

	.text {
		margin: 15rpx 10rpx;
		padding: 0 10rpx;
		/*background-color: #ebebeb;*/
		text-align: center;
		color: #9b9b9b;
		font-size: 26rpx;
	}
</style>
