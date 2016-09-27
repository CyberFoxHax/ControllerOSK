using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace ControllerOSK.Controls {
	public partial class DisplayInputControl{

		private void IsActiveChanged(InputBlock sender) {
		}

		public void SetSkin(string skinDir) {
			var jsonFile = Directory.GetFiles(skinDir)
				.FirstOrDefault(p => Path.GetExtension(p) == ".json");

			if (jsonFile == null) return;
			var contents = File.ReadAllText(jsonFile);
			var skinSettings = Newtonsoft.Json.JsonConvert.DeserializeObject<JsonModels.StyleModel>(contents);
			skinSettings.SetDefaults();

			var children = Children.OfType<InputBlock>().ToArray();

			foreach (var inputBlock in children){
				inputBlock.IsActiveChanged -= IsActiveChanged;
				inputBlock.IsActiveChanged += IsActiveChanged;
			}

			var angleSeq = new[]{
				children[5], children[8], children[7],
				children[6],			  children[3],
				children[0], children[1], children[2]
			};

			Width  = (double) skinSettings.RootSize;
			Height = (double) skinSettings.RootSize;

			var center = new Vector2(
				(float)(skinSettings.RootSize / 2),
				(float)(skinSettings.RootSize / 2)
			);
			Canvas.SetTop (children[4], (double)(center.Y - skinSettings.CircleSize / 2));
			Canvas.SetLeft(children[4], (double)(center.X - skinSettings.CircleSize / 2));

			const double tau = Math.PI * 2;

			for (var i = 0; i < angleSeq.Length; i++) {
				var angle = tau / angleSeq.Length * i;
				Canvas.SetTop (angleSeq[i], (double)(center.Y + Math.Sin(angle) * skinSettings.CircleDistanceFromCenter - skinSettings.CircleSize / 2));
				Canvas.SetLeft(angleSeq[i], (double)(center.X + Math.Cos(angle) * skinSettings.CircleDistanceFromCenter - skinSettings.CircleSize / 2));
			}

			foreach (var item in Wrapper.Resources.OfType<Style>())
				item.Resources.Clear();
			Wrapper.Resources.Clear();

			var bindingParentOfTypeInputBlock = new Binding("IsActive"){
				RelativeSource = new RelativeSource{
					AncestorType = typeof(InputBlock)
				}
			};

			var backgroundImage = System.Drawing.Image.FromFile(Path.Combine(skinDir, skinSettings.BackgroundImage));
			var cursorImage = System.Drawing.Image.FromFile(Path.Combine(skinDir, skinSettings.CursorImage));

			Wrapper.Background = new ImageBrush(Utils.BitmapToBitmapSource(backgroundImage));

			Wrapper.Resources.Add(typeof(InputBlock), new Style {
				Resources = {
					{
						typeof(Label),
						new Style{
							Triggers = {
								new DataTrigger{
									Binding = bindingParentOfTypeInputBlock,
									Value = true,
									Setters = {
										new Setter(FontSizeProperty,   skinSettings.ActiveTextStyle.FontSize),
										new Setter(FontFamilyProperty, new FontFamily(skinSettings.ActiveTextStyle.FontFace)),
										new Setter(FontWeightProperty, skinSettings.ActiveTextStyle.Bold?FontWeights.Black:FontWeights.Regular),
										new Setter(ForegroundProperty, new BrushConverter().ConvertFromString(skinSettings.ActiveTextStyle.Color))
									}
								}
							},
							Setters = {
								new Setter(HorizontalAlignmentProperty	, HorizontalAlignment.Center), 
								new Setter(VerticalAlignmentProperty	, VerticalAlignment.Center), 

								new Setter(FontSizeProperty	,   skinSettings.NormalTextStyle.FontSize),
								new Setter(FontFamilyProperty, new FontFamily(skinSettings.NormalTextStyle.FontFace)),
								new Setter(FontWeightProperty, FontWeight.FromOpenTypeWeight(skinSettings.NormalTextStyle.Bold?999:1)),
								new Setter(ForegroundProperty,   new BrushConverter().ConvertFromString(skinSettings.NormalTextStyle.Color))
							}
						}
					},
					{
						typeof(Grid),
						new Style{
							Triggers ={
								new DataTrigger{
									Binding = bindingParentOfTypeInputBlock,
									Value = true,
									Setters = {
										new Setter(BackgroundProperty, new ImageBrush(Utils.BitmapToBitmapSource(cursorImage))),
									}
								}
							},
							Setters ={
							}
						}
					}
				},
				Triggers = {
					new DataTrigger{
						Binding = new Binding("IsActive"){
							RelativeSource = RelativeSource.Self
						},
						Value = true,
						Setters = {
							new Setter(WidthProperty , skinSettings.ActiveCircleSize),
							new Setter(HeightProperty, skinSettings.ActiveCircleSize),
							new Setter(InputBlock.DistanceFromCenterProperty, skinSettings.ActiveCircleSize/2 - skinSettings.ActiveCirclePadding),
							new Setter(MarginProperty, new Thickness(
								(double) -(skinSettings.ActiveCircleSize - skinSettings.CircleSize)/2,
								(double) -(skinSettings.ActiveCircleSize - skinSettings.CircleSize)/2,
								0,
								0
							)),
						}
					}
				},
				Setters = {
					new Setter(WidthProperty , skinSettings.CircleSize ),
					new Setter(HeightProperty, skinSettings.CircleSize ),
					new Setter(InputBlock.DistanceFromCenterProperty, skinSettings.CircleSize/2 - skinSettings.CirclePadding)
				}
			});
		}
	}
}
