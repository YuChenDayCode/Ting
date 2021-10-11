<template>
	<view class="content">
		<view class="tag-view" v-for="(item,index) in Catalog" :key="index">
			<uni-tag :text="item.catalogname" @click="Player(item.link)"></uni-tag>
		</view>
	</view>
</template>

<script>
	export default {
		data() {
			return {
				bookid: "",
				Catalog: []
			}
		},
		onLoad(option) {
			this.bookid = option.id;
			uni.setNavigationBarTitle({
				title: option.name + '- 目录'
			});
			this.GetCatalog();

		},
		methods: {
			GetCatalog() {
				let _this = this;
				if (!_this.bookid) _this.bookid = "/mp3/2996.html";
				uni.request({
					url: '/api/Ting/GetCatalog',
					data: {
						moudle: _this.bookid
					},
					complete: (res) => {
						if (res.data.status) {
							_this.Catalog = res.data;
						}
					}
				});
			},
			Player(link) {
				uni.navigateTo({
					url: "/pages/player/player?id=" + link
				});
			}
		}
	}
</script>

<style>
	.content {
		display: flex;
		flex-direction: row;
		justify-content: flex-start;
		flex-wrap: wrap;
		padding: 10rpx;
	}

	.tag-view {
		flex-direction: column;
		margin: 15rpx;
		justify-content: center;
	}
</style>
