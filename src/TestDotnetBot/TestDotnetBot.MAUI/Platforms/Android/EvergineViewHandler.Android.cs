using TestDotnetBot.MAUI.Evergine;
using Evergine.AndroidView;
using Evergine.Common.Graphics;
using Evergine.Framework.Services;
using Evergine.Vulkan;
using TestDotnetBot;
using Microsoft.Maui.Handlers;

namespace TestDotnetBot.MAUI.Evergine
{
    public partial class EvergineViewHandler : ViewHandler<EvergineView, AndroidSurfaceView>
    {
        private AndroidSurface androidSurface;
        private AndroidWindowsSystem windowsSystem;

        public EvergineViewHandler(IPropertyMapper mapper, CommandMapper commandMapper = null)
           : base(mapper, commandMapper)
        {
        }

        public static void MapApplication(EvergineViewHandler handler, EvergineView evergineView)
        {
            handler.UpdateApplication(evergineView, evergineView.DisplayName);
        }

        internal void UpdateApplication(EvergineView view, string displayName)
        {
            if (view.Application is null)
            {
                return;
            }

            // Register Windows system
            view.Application.Container.RegisterInstance(this.windowsSystem);

            // Creates XAudio device
            var xaudio = new global::Evergine.OpenAL.ALAudioDevice();
            view.Application.Container.RegisterInstance(xaudio);

            System.Diagnostics.Stopwatch clockTimer = System.Diagnostics.Stopwatch.StartNew();
            this.windowsSystem.Run(
            () =>
            {
                this.ConfigureGraphicsContext(view.Application as TestDotnetBot.MyApplication, this.androidSurface);
                view.Application.Initialize();
            },
            () =>
            {
                var gameTime = clockTimer.Elapsed;
                clockTimer.Restart();

                view.Application.UpdateFrame(gameTime);
                view.Application.DrawFrame(gameTime);
            });
        }

        protected override AndroidSurfaceView CreatePlatformView()
        {
            this.windowsSystem = new AndroidWindowsSystem(this.Context);
            this.androidSurface = this.windowsSystem.CreateSurface(0, 0) as AndroidSurface;
            return this.androidSurface.NativeSurface;
        }

        private void ConfigureGraphicsContext(MyApplication application, Surface surface)
        {
            var graphicsContext = new VKGraphicsContext();
            graphicsContext.CreateDevice();
            SwapChainDescription swapChainDescription = new SwapChainDescription()
            {
                SurfaceInfo = surface.SurfaceInfo,
                Width = surface.Width,
                Height = surface.Height,
                ColorTargetFormat = PixelFormat.R8G8B8A8_UNorm,
                ColorTargetFlags = TextureFlags.RenderTarget | TextureFlags.ShaderResource,
                DepthStencilTargetFormat = PixelFormat.D24_UNorm_S8_UInt,
                DepthStencilTargetFlags = TextureFlags.DepthStencil,
                SampleCount = TextureSampleCount.None,
                IsWindowed = true,
                RefreshRate = 60,
            };
            var swapChain = graphicsContext.CreateSwapChain(swapChainDescription);
            swapChain.VerticalSync = true;

            var graphicsPresenter = application.Container.Resolve<GraphicsPresenter>();
            var firstDisplay = new global::Evergine.Framework.Graphics.Display(surface, swapChain);
            graphicsPresenter.AddDisplay("DefaultDisplay", firstDisplay);

            application.Container.RegisterInstance(graphicsContext);

            surface.OnScreenSizeChanged += (_, args) =>
            {
                swapChain.ResizeSwapChain(args.Height, args.Width);
            };
        }
    }
}
