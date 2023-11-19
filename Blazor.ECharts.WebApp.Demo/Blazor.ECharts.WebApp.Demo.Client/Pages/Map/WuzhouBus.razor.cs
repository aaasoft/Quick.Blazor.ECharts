﻿using Blazor.ECharts.Options;
using Blazor.ECharts.Options.Enum;
using Blazor.ECharts.WebApp.Demo.Client.Utils;
using Microsoft.AspNetCore.Components;
using L = Blazor.ECharts.Options.Series.Lines;

namespace Blazor.ECharts.WebApp.Demo.Client.Pages.Map
{
    [RouteName("梧州公交线路图")]
    public partial class WuzhouBus
    {
        private EChartsOption<L.Lines> Option1;
        [Inject]
        public HttpClient Http { get; set; }
        [Inject]
        public NavigationManager NavigationManager { get; set; }
        public record BusLinesData
        {
            public List<double[]> Coords { get; set; }
            public LineStyle LineStyle { get; set; }
        }

        protected async override Task OnInitializedAsync()
        {
            //通过csv获取line坐标
            Http.BaseAddress = new Uri(NavigationManager.BaseUri);
            string csv = await Http.GetStringAsync("sample-data/wuzhou_lines.csv");
            string[] rows = csv.Split('\n');
            List<BusLinesData> datas = [];
            foreach (var row in rows)
            {
                string[] cols = row.Split(",\"");

                if (cols.Length <= 1)
                    continue;

                string[] items = cols[1].Replace("\",", "").Trim().Split(';');
                List<double[]> points = [];
                for (int i = 0; i < items.Length - 1; i++)
                {
                    string[] point = items[i].Split(',');
                    double lng = Convert.ToDouble(point[0]);
                    double lat = Convert.ToDouble(point[1]);
                    //转换为百度地图坐标系
                    MapConverter.GCJ02ToBD09(lat, lng, out double blat, out double blng);
                    points.Add([blng, blat]);
                }

                datas.Add(new()
                {
                    Coords = points,
                    LineStyle = new()
                    {
                        Color = Util.GetRandomColor()
                    }
                });
            }

            Option1 = new()
            {
                Bmap = new()
                {
                    Center = [111.34, 23.51],
                    Zoom = 13,
                    Roam = true,
                    MapStyle = new()
                    {
                        StyleJson =
                        [
                            new()
                            {
                                FeatureType = "water",
                                ElementType = "all",
                                Stylers = new()
                                {
                                    Color = "#031628"
                                }
                            },
                            new()
                            {
                                FeatureType = "land",
                                ElementType = "geometry",
                                Stylers = new()
                                {
                                    Color = "#000102"
                                }
                            },
                            new()
                            {
                                FeatureType = "highway",
                                ElementType = "all",
                                Stylers = new()
                                {
                                    Visibility = "off"
                                }
                            },
                            new()
                            {
                                FeatureType = "arterial",
                                ElementType = "geometry.fill",
                                Stylers = new()
                                {
                                    Color = "#000000"
                                }
                            },
                            new()
                            {
                                FeatureType = "arterial",
                                ElementType = "geometry.stroke",
                                Stylers = new()
                                {
                                    Color = "#0b3d51"
                                }
                            },
                            new()
                            {
                                FeatureType = "local",
                                ElementType = "geometry",
                                Stylers = new()
                                {
                                    Color = "#000000"
                                }
                            },
                            new()
                            {
                                FeatureType = "railway",
                                ElementType = "geometry.fill",
                                Stylers = new()
                                {
                                    Color = "#000000"
                                }
                            },
                            new()
                            {
                                FeatureType = "railway",
                                ElementType = "geometry.stroke",
                                Stylers = new()
                                {
                                    Color = "#08304b"
                                }
                            },
                            new()
                            {
                                FeatureType = "subway",
                                ElementType = "geometry",
                                Stylers = new()
                                {
                                    Lightness = -70
                                }
                            },
                            new()
                            {
                                FeatureType = "building",
                                ElementType = "geometry.fill",
                                Stylers = new()
                                {
                                    Color = "#000000"
                                }
                            },
                            new()
                            {
                                FeatureType = "all",
                                ElementType = "labels.text.fill",
                                Stylers = new()
                                {
                                    Color = "#857f7f"
                                }
                            },
                            new()
                            {
                                FeatureType = "all",
                                ElementType = "labels.text.stroke",
                                Stylers = new()
                                {
                                    Color = "#000000"
                                }
                            },
                            new()
                            {
                                FeatureType = "building",
                                ElementType = "geometry",
                                Stylers = new()
                                {
                                    Color = "#022338"
                                }
                            },
                            new()
                            {
                                FeatureType = "green",
                                ElementType = "geometry",
                                Stylers = new()
                                {
                                    Color = "#062032"
                                }
                            },
                            new()
                            {
                                FeatureType = "boundary",
                                ElementType = "all",
                                Stylers = new()
                                {
                                    Color = "#465b6c"
                                }
                            },
                            new()
                            {
                                FeatureType = "manmade",
                                ElementType = "all",
                                Stylers = new()
                                {
                                    Color = "#022338"
                                }
                            },
                            new()
                            {
                                FeatureType = "label",
                                ElementType = "all",
                                Stylers = new()
                                {
                                    Visibility = "off"
                                }
                            }
                        ]
                    }
                },
                Series =
                [
                    new L.Lines()
                    {
                        CoordinateSystem = CoordinateSystem.Bmap,
                        Polyline = true,
                        Data = datas,
                        Silent = true,
                        LineStyle = new()
                        {
                            Opacity = 0.2,
                            Width = 1
                        },
                        ProgressiveThreshold = 500,
                        Progressive = 200
                    },
                    new L.Lines()
                    {
                        CoordinateSystem = CoordinateSystem.Bmap,
                        Polyline = true,
                        Data = datas,
                        LineStyle = new()
                        {
                            Width = 0
                        },
                        Effect = new()
                        {
                            ConstantSpeed = 20,
                            Show = true,
                            TrailLength = 0.1,
                            SymbolSize = 1.5
                        },
                        Zlevel = 1
                    }
                ]
            };

            await base.OnInitializedAsync();
        }
    }
}