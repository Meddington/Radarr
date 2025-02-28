using System.Collections.Generic;
using System.IO;
using System.Linq;
using NzbDrone.Core.Configuration;
using NzbDrone.Core.MediaFiles;
using NzbDrone.Core.Movies;
using NzbDrone.Core.ThingiProvider;

namespace NzbDrone.Core.Notifications.Webhook
{
    public abstract class WebhookBase<TSettings> : NotificationBase<TSettings>
        where TSettings : IProviderConfig, new()
    {
        private readonly IConfigFileProvider _configFileProvider;

        protected WebhookBase(IConfigFileProvider configFileProvider)
            : base()
        {
            _configFileProvider = configFileProvider;
        }

        protected WebhookGrabPayload BuildOnGrabPayload(GrabMessage message)
        {
            var remoteMovie = message.RemoteMovie;
            var quality = message.Quality;

            return new WebhookGrabPayload
            {
                EventType = WebhookEventType.Grab,
                InstanceName = _configFileProvider.InstanceName,
                Movie = new WebhookMovie(message.Movie),
                RemoteMovie = new WebhookRemoteMovie(remoteMovie),
                Release = new WebhookRelease(quality, remoteMovie),
                DownloadClient = message.DownloadClientName,
                DownloadClientType = message.DownloadClientType,
                DownloadId = message.DownloadId
            };
        }

        protected WebhookImportPayload BuildOnDownloadPayload(DownloadMessage message)
        {
            var movieFile = message.MovieFile;

            var payload = new WebhookImportPayload
            {
                EventType = WebhookEventType.Download,
                InstanceName = _configFileProvider.InstanceName,
                Movie = new WebhookMovie(message.Movie),
                RemoteMovie = new WebhookRemoteMovie(message.Movie),
                MovieFile = new WebhookMovieFile(movieFile),
                IsUpgrade = message.OldMovieFiles.Any(),
                DownloadClient = message.DownloadClientInfo?.Name,
                DownloadClientType = message.DownloadClientInfo?.Type,
                DownloadId = message.DownloadId
            };

            if (message.OldMovieFiles.Any())
            {
                payload.DeletedFiles = message.OldMovieFiles.ConvertAll(x =>
                    new WebhookMovieFile(x)
                    {
                        Path = Path.Combine(message.Movie.Path, x.RelativePath)
                    });
            }

            return payload;
        }

        protected WebhookAddedPayload BuildOnMovieAdded(Movie movie)
        {
            return new WebhookAddedPayload
            {
                EventType = WebhookEventType.MovieAdded,
                InstanceName = _configFileProvider.InstanceName,
                Movie = new WebhookMovie(movie),
                AddMethod = movie.AddOptions.AddMethod
            };
        }

        protected WebhookMovieFileDeletePayload BuildOnMovieFileDelete(MovieFileDeleteMessage deleteMessage)
        {
            return new WebhookMovieFileDeletePayload
            {
                EventType = WebhookEventType.MovieFileDelete,
                InstanceName = _configFileProvider.InstanceName,
                Movie = new WebhookMovie(deleteMessage.Movie),
                MovieFile = new WebhookMovieFile(deleteMessage.MovieFile),
                DeleteReason = deleteMessage.Reason
            };
        }

        protected WebhookMovieDeletePayload BuildOnMovieDelete(MovieDeleteMessage deleteMessage)
        {
            var payload = new WebhookMovieDeletePayload
            {
                EventType = WebhookEventType.MovieDelete,
                InstanceName = _configFileProvider.InstanceName,
                Movie = new WebhookMovie(deleteMessage.Movie),
                DeletedFiles = deleteMessage.DeletedFiles
            };

            if (deleteMessage.DeletedFiles && deleteMessage.Movie.MovieFile != null)
            {
                payload.MovieFolderSize = deleteMessage.Movie.MovieFile.Size;
            }

            return payload;
        }

        protected WebhookRenamePayload BuildOnRenamePayload(Movie movie, List<RenamedMovieFile> renamedFiles)
        {
            return new WebhookRenamePayload
            {
                EventType = WebhookEventType.Rename,
                InstanceName = _configFileProvider.InstanceName,
                Movie = new WebhookMovie(movie),
                RenamedMovieFiles = renamedFiles.ConvertAll(x => new WebhookRenamedMovieFile(x))
            };
        }

        protected WebhookHealthPayload BuildHealthPayload(HealthCheck.HealthCheck healthCheck)
        {
            return new WebhookHealthPayload
            {
                EventType = WebhookEventType.Health,
                InstanceName = _configFileProvider.InstanceName,
                Level = healthCheck.Type,
                Message = healthCheck.Message,
                Type = healthCheck.Source.Name,
                WikiUrl = healthCheck.WikiUrl?.ToString()
            };
        }

        protected WebhookApplicationUpdatePayload BuildApplicationUpdatePayload(ApplicationUpdateMessage updateMessage)
        {
            return new WebhookApplicationUpdatePayload
            {
                EventType = WebhookEventType.ApplicationUpdate,
                InstanceName = _configFileProvider.InstanceName,
                Message = updateMessage.Message,
                PreviousVersion = updateMessage.PreviousVersion.ToString(),
                NewVersion = updateMessage.NewVersion.ToString()
            };
        }

        protected WebhookPayload BuildTestPayload()
        {
            return new WebhookGrabPayload
            {
                EventType = WebhookEventType.Test,
                InstanceName = _configFileProvider.InstanceName,
                Movie = new WebhookMovie
                {
                    Id = 1,
                    Title = "Test Title",
                    Year = 1970,
                    FolderPath = "C:\\testpath",
                    ReleaseDate = "1970-01-01"
                },
                RemoteMovie = new WebhookRemoteMovie
                {
                    TmdbId = 1234,
                    ImdbId = "5678",
                    Title = "Test title",
                    Year = 1970
                },
                Release = new WebhookRelease
                {
                    Indexer = "Test Indexer",
                    Quality = "Test Quality",
                    QualityVersion = 1,
                    ReleaseGroup = "Test Group",
                    ReleaseTitle = "Test Title",
                    Size = 9999999
                }
            };
        }
    }
}
