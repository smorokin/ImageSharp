// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

using Microsoft.DotNet.RemoteExecutor;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Tests.TestUtilities;
using Xunit;

// ReSharper disable InconsistentNaming
namespace SixLabors.ImageSharp.Tests.Formats.Jpg
{
    [Trait("Format", "Jpg")]
    public partial class JpegDecoderTests
    {
        public const string DecodeProgressiveJpegOutputName = "DecodeProgressiveJpeg";

        [Theory]
        [WithFileCollection(nameof(ProgressiveTestJpegs), PixelTypes.Rgb24)]
        public void DecodeProgressiveJpeg<TPixel>(TestImageProvider<TPixel> provider)
            where TPixel : unmanaged, IPixel<TPixel>
        {
            using Image<TPixel> image = provider.GetImage(JpegDecoder);
            image.DebugSave(provider);

            provider.Utility.TestName = DecodeProgressiveJpegOutputName;
            image.CompareToReferenceOutput(
                GetImageComparer(provider),
                provider,
                appendPixelTypeToFileName: false);
        }

        [Theory]
        [WithFile(TestImages.Jpeg.Progressive.Progress, PixelTypes.Rgb24)]
        public void DecodeProgressiveJpeg_WithLimitedAllocatorBufferCapacity(TestImageProvider<Rgb24> provider)
        {
            static void RunTest(string providerDump, string nonContiguousBuffersStr)
            {
                TestImageProvider<Rgb24> provider =
                    BasicSerializer.Deserialize<TestImageProvider<Rgb24>>(providerDump);

                provider.LimitAllocatorBufferCapacity().InBytesSqrt(200);

                using Image<Rgb24> image = provider.GetImage(JpegDecoder);
                image.DebugSave(provider, nonContiguousBuffersStr);

                provider.Utility.TestName = DecodeProgressiveJpegOutputName;
                image.CompareToReferenceOutput(
                    GetImageComparer(provider),
                    provider,
                    appendPixelTypeToFileName: false);
            }

            string providerDump = BasicSerializer.Serialize(provider);

            RemoteExecutor.Invoke(
                RunTest,
                providerDump,
                "Disco").Dispose();
        }
    }
}
