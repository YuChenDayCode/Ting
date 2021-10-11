<template>
	<view>
		<view class="audio-player">
			<uni-view class="txt-title">{{part.name}}</uni-view>
			<uni-view class="audio-wrapper">
				<uni-view class="audio-number">{{player.currentTimestr}}</uni-view>
				<slider class="audio-slider" :value="player.currentTime" :disabled="!player.canPlay"
					@change="sliderChange" min="0" :max="player.duration" block-size="15" activeColor="#C0C0C0"
					backgroundColor="#FFCC33" block-color="#8A6DE9" />
				<uni-view class="audio-number">{{player.durationstr}}</uni-view>
			</uni-view>
			<view class="audio-button-box">
				<!-- 上一首 -->
				<image :src="require('../../static/go.png')" class="prevplay" @click="switchpart(1)" mode="widthFix">
				</image>
				<div class="playbox">
					<image :src="require('../../static/loading2.png')" v-if="!player.canPlay" class="play loading">
					</image>
					<template v-else>
						<!-- 播放 -->
						<image :src="require('../../static/playbtn2.png')" alt="" class="play" @click="playswitch(1)"
							v-if="player.playing"></image>
						<!-- 暂停 -->
						<image :src="require('../../static/pausebtn2.png')" alt="" class="pause" @click="playswitch(2)"
							v-else></image>
					</template>
				</div>
				<!-- 下一首 -->
				<image :src="require('../../static/go.png')" class="nextplay" @click="switchpart(2)" mode="widthFix">
				</image>
			</view>
		</view>
	</view>
</template>

<script>
	export default {
		data() {
			return {
				InnerAuido: null,
				player: {
					canPlay: false, //加载完成 可以播放
					playing: false,
					duration: 0,
					durationstr: "0",
					currentTime: 0,
					currentTimestr: 0
				},
				part: {
					id: 0,
					url: "",
					name: "",
					preepis: 0,
					thisepis: '',
					nextepis: '',
					totleepis: 0
				}
			}
		},
		onLoad(option) {
			this.InnerAuido = uni.createInnerAudioContext();
			console.log(option);
			let vid = option.id.match(/\d+/)[0];
			//uni.clearStorage(vid);
			let storage = uni.getStorageSync(vid+'_history');
			console.log(storage);
			if (storage) {
				this.part = storage;
				this.play(this.part.url);
				console.log('使用缓存播放');
			} else {
				this.GetPlayerUrl(option.id);
			}

			//let urlss = 'http://audio.xmcdn.com/group31/M00/37/2D/wKgJX1mC_KOTdsPSAGPjnuWsJFI568.m4a';
			//this.play(urlss);
		},
		methods: {
			play(source) {
				this.InnerAuido.autoplay = true;
				this.InnerAuido.src = source;
				this.InnerAuido.onPlay(() => {
					console.log('开始播放');
				});
				this.InnerAuido.onCanplay(() => {
					console.log('load success');
					this.player.duration = parseInt(this.InnerAuido.duration);
					this.player.durationstr = this.FillZore(parseInt(this.InnerAuido.duration / 60) + ":" +
						parseInt(
							this.InnerAuido.duration % 60));
					this.player.canPlay = true;
				});
				this.InnerAuido.onEnded(()=>{
					this.GetPlayerUrl(this.part.nextepis);
				});
				this.InnerAuido.onTimeUpdate(() => {
					this.player.currentTime = parseInt(this.InnerAuido.currentTime);
					this.player.currentTimestr = this.FillZore(parseInt(this.InnerAuido.currentTime / 60) + ":" +
						parseInt(
							this.InnerAuido.currentTime % 60));
				});
				this.InnerAuido.onError((res) => {
					console.log(res);
				});
			},
			switchpart(flag) {
				if (!this.player.canPlay) return;
				console.log(this.part.nextepis);
				if (flag == 1) {
					if (this.part.preepis == this.part.thisepis) {
						uni.showToast({
							title: '已经是第一集',
							icon: "none",
							duration: 3000
						});
						return;
					};
					this.GetPlayerUrl(this.part.preepis);
				}
				if (flag == 2) {
					if (this.part.nextepis == this.part.thisepis) {
						uni.showToast({
							title: '已经是最后一集',
							icon: "none",
							duration: 3000
						});
						return;
					};
					this.GetPlayerUrl(this.part.nextepis);
				}
			},
			playswitch(i) {
				//this.InnerAuido.paused true 表示暂停或停止，false 表示正在播放
				this.player.playing = !this.InnerAuido.paused;
				if (this.InnerAuido.paused) this.InnerAuido.play();
				else this.InnerAuido.pause();
			},
			FillZore(value) {
				let sf = value.split(':');
				let left = sf[0],
					right = sf[1];
				if (left.length < 2) left = '0' + sf[0];
				if (right.length < 2) right = '0' + sf[1];
				return left + ':' + right;
			},
			sliderChange(e) {
				this.InnerAuido.seek(e.detail.value);
				this.player.currentTime = e.detail.value;
			},
			GetPlayerUrl(catalogurl) {
				let _this = this;
				//catalogurl = "video/2038-0-0.html";

				uni.request({
					url: '/api/Ting/GetPlayUrl',
					data: {
						moudle: catalogurl
					},
					success: (res) => {
						console.log(res);
						let data = res.data;

						uni.setNavigationBarTitle({
							title: data.title
						});
						let u = _this.FonHen_JieMa(data.url);
						console.log(u);
						let play_url = _this.resolve_tc(u);

						_this.part = {
							id: data.vid,
							url: play_url,
							name: data.title,
							preepis: _this.generate_epis(catalogurl, u[1]).pre,
							thisepis: catalogurl,
							nextepis: _this.generate_epis(catalogurl, u[1]).next,
							totleepis: u[1]
						};
						console.log(_this.part);
						_this.set_memoryCache(data.vid+'_history', _this.part)
						_this.play(play_url);
					},
					fail(err) {
						console.log(err);
					}
				});
			},
			FonHen_JieMa(u) {
				var tArr = u.split("*");
				var str = '';
				for (var i = 1, n = tArr.length; i < n; i++) {
					str += String.fromCharCode(tArr[i]);
				}
				return str.split('&');
			},
			resolve_tc(obj) {
				let _this = this;
				let result_url = '';
				if (obj[2] == 'tc') {
					let url = obj[0].split('/');
					let urlstr = url[0] + '/' + url[1] + '/play_' + url[1] + '_' + url[2] + '.htm';
					uni.request({
						url: 'http://m.ting56.com/player/tingchina.php',
						methods: 'POST',
						data: {
							url: urlstr
						},
						header: {
							'content-type': 'application/x-www-form-urlencoded; charset=UTF-8'
						},
						complete(res) {
							if (res.statusCode != 200) {
								result_url = res.data;
							} else {
								uni.showModal({
									title: "提 示",
									content: res.data,
									showCancel: false,
									confirmText: "确定"
								});
							}
						}
					});
				} else {
					result_url = obj[0];
				}
				return result_url;
			},
			generate_epis(thisepis, totle) {
				let pre = thisepis.replace(/\d+(?=\D*$)/g, function(macth) {
					let epis = parseInt(macth);
					if (epis == 0) return macth; //是否第一集
					return epis - 1;
				});
				let next = thisepis.replace(/\d+(?=\D*$)/g, function(macth) {
					let epis = parseInt(macth);
					if (epis == totle) return macth; //是否最后一集
					return epis + 1;
				});
				return {
					pre: pre,
					next: next
				};
			},
			set_memoryCache(key, data) {
				uni.setStorage({
					key: key,
					data: data,
					success: function() {
						console.log('cache success');
					}
				});
			}
		}
	}
</script>

<style>
	.audio-player {
		border-radius: 15rpx;
		box-shadow: 0 0 15rpx 2rpx rgba(0, 0, 0, 0.2);
		margin: 30rpx;
		padding: 20rpx 0 20rpx 0;
	}

	.txt-title {
		font-weight: 600;
		font-size: 120%;
		text-align: center;
	}

	.audio-wrapper {
		display: flex;
		align-items: center;
		width: 90%;
		margin: 30rpx auto;
	}

	.audio-number {
		display: block;
		font-size: 24upx;
		line-height: 1;
		color: #333;
	}

	.audio-slider {
		flex: 1;
		display: block;
		margin: 0 30rpx 0 35rpx;
	}

	.audio-button-box {
		display: flex;
		align-items: center;
		margin: 40rpx auto 0;
		justify-content: space-around;
		height: 100rpx
	}

	.playbox {
		width: 100rpx;
		height: 100rpx;
		display: flex;
		align-items: center;
		justify-content: center;
	}

	.play,
	.pause {
		width: 100rpx;
		height: 100rpx;

		&.loading {
			width: 80rpx;
			height: 80rpx;
			animation: rotating 2s linear infinite;
		}
	}

	.prevbtn,
	.nextbtn {
		width: 40rpx;
		height: 40rpx;
	}

	.prevplay {
		width: 40rpx;
		height: 40rpx;
		transform: rotateZ(180deg);
	}

	.nextplay {
		width: 40rpx;
		height: 40rpx;
	}
</style>
