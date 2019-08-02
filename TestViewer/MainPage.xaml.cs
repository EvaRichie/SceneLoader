// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Numerics;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Composition;
using Windows.UI.Composition.Scenes;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;
using SceneLoaderComponent;
using CameraComponent;
using Windows.ApplicationModel.Appointments.DataProvider;

namespace TestViewer
{
    /// <summary>
    /// A page that generates a gltf image using the SceneLoaderComponent.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        readonly Compositor _compositor;
        readonly SceneVisual _sceneVisual;
        private Viewport _viewport;

        public MainPage()
        {
            this.InitializeComponent();

            _compositor = Window.Current.Compositor;

            var root = _compositor.CreateContainerVisual();

            root.Size = new Vector2(1000, 1000);
            ElementCompositionPreview.SetElementChildVisual(this, root);

            _sceneVisual = SceneVisual.Create(_compositor);
            root.Children.InsertAtTop(_sceneVisual);

            _sceneVisual.Offset = new Vector3(300, 300, 0);
            _sceneVisual.RotationAxis = new Vector3(0, 1, 0);

            _viewport = new Viewport(_sceneVisual);
            OrbitalCamera camera = new OrbitalCamera();
            _viewport.Camera = camera;
            camera.Longitude = MathF.PI / 2f;
            camera.Latitude = MathF.PI / 2f;
            camera.Projection = new PerspectiveProjection();
        }

        async Task<SceneNode> LoadGLTF(Uri uri)
        {
            var storageFile = await StorageFile.GetFileFromApplicationUriAsync(uri);
            var buffer = await FileIO.ReadBufferAsync(storageFile);

            var loader = new SceneLoader();
            return loader.Load(buffer, _compositor);
        }

        async void Page_Loaded(object sender, RoutedEventArgs e) => _sceneVisual.Root = await LoadGLTF(new Uri("ms-appx:///Assets/DamagedHelmet.gltf"));
    }
}
