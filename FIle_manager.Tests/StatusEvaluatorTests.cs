using File_manager.Models;
using File_manager.Services;
using System;
using System.IO;
using Xunit;

namespace File_manager.Tests
{
    public class StatusEvaluatorTests
    {
        private readonly StatusEvaluator _sut = new();

        private static AssetMetadata Baseline(
            long size = 100,
            DateTime? registeredTime = null,
            DateTime? firstSeen = null) => new()
            {
                RegisteredTime = registeredTime ?? DateTime.Now.AddSeconds(-10),
                RegisteredSize = size,
                FirstSeenTime = firstSeen ?? DateTime.Now
            };

        // ── CalculateStatus ──────────────────────────────────────────

        [Fact]
        public void CalculateStatus_FileNotExists_ReturnsMissing()
        {
            var fake = new FileInfo("C:\\does_not_exist_xyz.txt");
            var result = _sut.CalculateStatus(fake, Baseline());
            Assert.Equal(FileStatus.Missing, result);
        }

        [Fact]
        public void CalculateStatus_UnchangedFile_ReturnsNew()
        {
            var tmp = Path.GetTempFileName();
            try
            {
                var fi = new FileInfo(tmp);
                var baseline = Baseline(fi.Length, registeredTime: fi.LastWriteTime);
                var result = _sut.CalculateStatus(fi, baseline);
                Assert.Equal(FileStatus.New, result);
            }
            finally { File.Delete(tmp); }
        }

        [Fact]
        public void CalculateStatus_SizeChanged_ReturnsModified()
        {
            var tmp = Path.GetTempFileName();
            try
            {
                File.WriteAllText(tmp, "some content");
                var fi = new FileInfo(tmp);
                var baseline = Baseline(size: 1, registeredTime: fi.LastWriteTime);
                var result = _sut.CalculateStatus(fi, baseline);
                Assert.Equal(FileStatus.Modified, result);
            }
            finally { File.Delete(tmp); }
        }

        [Fact]
        public void CalculateStatus_ZeroSizeFile_ReturnsNew()
        {
            var tmp = Path.GetTempFileName();
            try
            {
                var fi = new FileInfo(tmp);
                var baseline = Baseline(size: 0, registeredTime: fi.LastWriteTime);
                var result = _sut.CalculateStatus(fi, baseline);
                Assert.Equal(FileStatus.New, result);
            }
            finally { File.Delete(tmp); }
        }

        [Fact]
        public void CalculateStatus_SizeChangedByOneByte_ReturnsModified()
        {
            var tmp = Path.GetTempFileName();
            try
            {
                File.WriteAllBytes(tmp, new byte[101]);
                var fi = new FileInfo(tmp);
                var baseline = Baseline(size: 100, registeredTime: fi.LastWriteTime);
                var result = _sut.CalculateStatus(fi, baseline);
                Assert.Equal(FileStatus.Modified, result);
            }
            finally { File.Delete(tmp); }
        }

        [Fact]
        public void CalculateStatus_TimeChanged_ReturnsModified()
        {
            var tmp = Path.GetTempFileName();
            try
            {
                var fi = new FileInfo(tmp);
                var baseline = Baseline(
                    size: fi.Length,
                    registeredTime: fi.LastWriteTime.AddSeconds(-60));
                var result = _sut.CalculateStatus(fi, baseline);
                Assert.Equal(FileStatus.Modified, result);
            }
            finally { File.Delete(tmp); }
        }

        // ── ResolveStatus — базові ───────────────────────────────────

        [Fact]
        public void ResolveStatus_FileMissing_ReturnsMissing()
        {
            var fake = new FileInfo("C:\\no_such_file_abc.txt");
            var result = _sut.ResolveStatus(fake, Baseline(), FileStatus.Approved);
            Assert.Equal(FileStatus.Missing, result);
        }

        [Fact]
        public void ResolveStatus_UnchangedFile_ReturnsNew()
        {
            var tmp = Path.GetTempFileName();
            try
            {
                var fi = new FileInfo(tmp);
                var baseline = Baseline(fi.Length, registeredTime: fi.LastWriteTime);
                var result = _sut.ResolveStatus(fi, baseline, FileStatus.New);
                Assert.Equal(FileStatus.New, result);
            }
            finally { File.Delete(tmp); }
        }

        // ── ResolveStatus — збережені статуси не змінюються ─────────

        [Theory]
        [InlineData(FileStatus.Approved)]
        [InlineData(FileStatus.Rejected)]
        [InlineData(FileStatus.Done)]
        public void ResolveStatus_UnchangedFile_PreservesManualStatus(FileStatus savedStatus)
        {
            var tmp = Path.GetTempFileName();
            try
            {
                var fi = new FileInfo(tmp);
                var baseline = Baseline(fi.Length, registeredTime: fi.LastWriteTime);
                var result = _sut.ResolveStatus(fi, baseline, savedStatus);
                Assert.Equal(savedStatus, result);
            }
            finally { File.Delete(tmp); }
        }

        // ── ResolveStatus — Modified перебиває будь-який статус ──────

        [Theory]
        [InlineData(FileStatus.Approved)]
        [InlineData(FileStatus.Rejected)]
        [InlineData(FileStatus.Done)]
        [InlineData(FileStatus.New)]
        public void ResolveStatus_SizeChanged_OverridesAnyStatus(FileStatus currentStatus)
        {
            var tmp = Path.GetTempFileName();
            try
            {
                File.WriteAllText(tmp, "changed content");
                var fi = new FileInfo(tmp);
                var baseline = Baseline(size: 1, registeredTime: fi.LastWriteTime);
                var result = _sut.ResolveStatus(fi, baseline, currentStatus);
                Assert.Equal(FileStatus.Modified, result);
            }
            finally { File.Delete(tmp); }
        }

        // ── ResolveStatus — Missing перебиває будь-який статус ───────

        [Theory]
        [InlineData(FileStatus.Approved)]
        [InlineData(FileStatus.Rejected)]
        [InlineData(FileStatus.Done)]
        [InlineData(FileStatus.New)]
        public void ResolveStatus_FileMissing_OverridesAnyStatus(FileStatus currentStatus)
        {
            var fake = new FileInfo("C:\\no_such_file_999.txt");
            var result = _sut.ResolveStatus(fake, Baseline(), currentStatus);
            Assert.Equal(FileStatus.Missing, result);
        }
    }
}