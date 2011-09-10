﻿using System;
using System.Collections.Generic;
using AutoMoq;
using FizzWare.NBuilder;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using NzbDrone.Core.Model;
using NzbDrone.Core.Model.Notification;
using NzbDrone.Core.Providers;
using NzbDrone.Core.Providers.Core;
using NzbDrone.Core.Providers.Indexer;
using NzbDrone.Core.Providers.Jobs;
using NzbDrone.Core.Repository;
using NzbDrone.Core.Repository.Quality;
using NzbDrone.Core.Test.Framework;

namespace NzbDrone.Core.Test
{
    [TestFixture]
    // ReSharper disable InconsistentNaming
    public class BannerDownloadJobTest : TestBase
    {
        [Test]
        public void BannerDownload_all()
        {
            //Setup
            var fakeSeries = Builder<Series>.CreateListOfSize(10)
                .Build();

            var mocker = new AutoMoqer(MockBehavior.Strict);

            var notification = new ProgressNotification("Banner Download");

            mocker.GetMock<SeriesProvider>()
                .Setup(c => c.GetAllSeries())
                .Returns(fakeSeries);

            mocker.GetMock<HttpProvider>()
                .Setup(s => s.DownloadFile(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(true);

            mocker.GetMock<DiskProvider>()
                .Setup(S => S.CreateDirectory(It.IsAny<string>()))
                .Returns("");

            //Act
            mocker.Resolve<BannerDownloadJob>().Start(notification, 0, 0);

            //Assert
            mocker.VerifyAllMocks();
            mocker.GetMock<HttpProvider>().Verify(s => s.DownloadFile(It.IsAny<string>(), It.IsAny<string>()),
                                                       Times.Exactly(fakeSeries.Count));
        }

        [Test]
        public void BannerDownload_some_null_BannerUrl()
        {
            //Setup
            var fakeSeries = Builder<Series>.CreateListOfSize(10)
                .WhereRandom(2)
                .Have(s => s.BannerUrl = null)
                .Build();

            var mocker = new AutoMoqer(MockBehavior.Strict);

            var notification = new ProgressNotification("Banner Download");

            mocker.GetMock<SeriesProvider>()
                .Setup(c => c.GetAllSeries())
                .Returns(fakeSeries);

            mocker.GetMock<HttpProvider>()
                .Setup(s => s.DownloadFile(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(true);

            mocker.GetMock<DiskProvider>()
                .Setup(S => S.CreateDirectory(It.IsAny<string>()))
                .Returns("");

            //Act
            mocker.Resolve<BannerDownloadJob>().Start(notification, 0, 0);

            //Assert
            mocker.VerifyAllMocks();
            mocker.GetMock<HttpProvider>().Verify(s => s.DownloadFile(It.IsAny<string>(), It.IsAny<string>()),
                                                       Times.Exactly(8));
        }

        [Test]
        public void BannerDownload_some_failed_download()
        {
            //Setup
            var fakeSeries = Builder<Series>.CreateListOfSize(10)
                .Build();

            const string path = @"C:\Users\mark.mcdowall\Dropbox\Visual Studio 2010\NzbDrone\NzbDrone.Core.Test\bin\Debug\Content\Images\Banners\";

            var mocker = new AutoMoqer(MockBehavior.Strict);

            var notification = new ProgressNotification("Banner Download");

            mocker.GetMock<SeriesProvider>()
                .Setup(c => c.GetAllSeries())
                .Returns(fakeSeries);

            mocker.GetMock<HttpProvider>()
                .Setup(s => s.DownloadFile(It.IsAny<string>(), path + "1.jpg"))
                .Returns(false);

            mocker.GetMock<HttpProvider>()
                .Setup(s => s.DownloadFile(It.IsAny<string>(), path + "2.jpg"))
                .Returns(true);

            mocker.GetMock<HttpProvider>()
                .Setup(s => s.DownloadFile(It.IsAny<string>(), path + "3.jpg"))
                .Returns(false);

            mocker.GetMock<HttpProvider>()
                .Setup(s => s.DownloadFile(It.IsAny<string>(), path + "4.jpg"))
                .Returns(true);

            mocker.GetMock<HttpProvider>()
                .Setup(s => s.DownloadFile(It.IsAny<string>(), path + "5.jpg"))
                .Returns(false);

            mocker.GetMock<HttpProvider>()
                .Setup(s => s.DownloadFile(It.IsAny<string>(), path + "6.jpg"))
                .Returns(true);

            mocker.GetMock<HttpProvider>()
                .Setup(s => s.DownloadFile(It.IsAny<string>(), path + "7.jpg"))
                .Returns(false);

            mocker.GetMock<HttpProvider>()
                .Setup(s => s.DownloadFile(It.IsAny<string>(), path + "8.jpg"))
                .Returns(true);

            mocker.GetMock<HttpProvider>()
                .Setup(s => s.DownloadFile(It.IsAny<string>(), path + "9.jpg"))
                .Returns(false);

            mocker.GetMock<HttpProvider>()
                .Setup(s => s.DownloadFile(It.IsAny<string>(), path + "10.jpg"))
                .Returns(true);

            mocker.GetMock<DiskProvider>()
                .Setup(S => S.CreateDirectory(It.IsAny<string>()))
                .Returns("");

            //Act
            mocker.Resolve<BannerDownloadJob>().Start(notification, 0, 0);

            //Assert
            mocker.VerifyAllMocks();
            mocker.GetMock<HttpProvider>().Verify(s => s.DownloadFile(It.IsAny<string>(), It.IsAny<string>()),
                                                       Times.Exactly(fakeSeries.Count));
        }

        [Test]
        public void BannerDownload_all_failed_download()
        {
            //Setup
            var fakeSeries = Builder<Series>.CreateListOfSize(10)
                .Build();

            var mocker = new AutoMoqer(MockBehavior.Strict);

            var notification = new ProgressNotification("Banner Download");

            mocker.GetMock<SeriesProvider>()
                .Setup(c => c.GetAllSeries())
                .Returns(fakeSeries);

            mocker.GetMock<HttpProvider>()
                .Setup(s => s.DownloadFile(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(false);

            mocker.GetMock<DiskProvider>()
                .Setup(S => S.CreateDirectory(It.IsAny<string>()))
                .Returns("");

            //Act
            mocker.Resolve<BannerDownloadJob>().Start(notification, 0, 0);

            //Assert
            mocker.VerifyAllMocks();
            mocker.GetMock<HttpProvider>().Verify(s => s.DownloadFile(It.IsAny<string>(), It.IsAny<string>()),
                                                       Times.Exactly(fakeSeries.Count));
        }

        [Test]
        public void BannerDownload_single_banner()
        {
            //Setup
            var fakeSeries = Builder<Series>.CreateNew()
                .With(s => s.SeriesId = 1)
                .Build();

            var mocker = new AutoMoqer(MockBehavior.Strict);

            var notification = new ProgressNotification("Banner Download");

            mocker.GetMock<SeriesProvider>()
                .Setup(c => c.GetSeries(1))
                .Returns(fakeSeries);

            mocker.GetMock<HttpProvider>()
                .Setup(s => s.DownloadFile(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(true);

            mocker.GetMock<DiskProvider>()
                .Setup(S => S.CreateDirectory(It.IsAny<string>()))
                .Returns("");

            //Act
            mocker.Resolve<BannerDownloadJob>().Start(notification, 1, 0);

            //Assert
            mocker.VerifyAllMocks();
            mocker.GetMock<HttpProvider>().Verify(s => s.DownloadFile(It.IsAny<string>(), It.IsAny<string>()),
                                                       Times.Once());
        }

        [Test]
        public void Download_Banner()
        {
            //Setup
            var fakeSeries = Builder<Series>.CreateNew()
                .With(s => s.SeriesId = 1)
                .Build();

            var mocker = new AutoMoqer(MockBehavior.Strict);

            var notification = new ProgressNotification("Banner Download");

            mocker.GetMock<HttpProvider>()
                .Setup(s => s.DownloadFile(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(true);

            //Act
            mocker.Resolve<BannerDownloadJob>().DownloadBanner(notification, fakeSeries);

            //Assert
            mocker.VerifyAllMocks();
            mocker.GetMock<HttpProvider>().Verify(s => s.DownloadFile(It.IsAny<string>(), It.IsAny<string>()),
                                                       Times.Once());
        }
    }
}