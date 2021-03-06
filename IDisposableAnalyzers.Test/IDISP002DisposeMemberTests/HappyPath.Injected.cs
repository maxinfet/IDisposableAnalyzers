namespace IDisposableAnalyzers.Test.IDISP002DisposeMemberTests
{
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    internal partial class HappyPath
    {
        internal class Injected
        {
            [Test]
            public void IgnoreAssignedWithCtorArgument()
            {
                var testCode = @"
namespace RoslynSandbox
{
    using System;

    public sealed class Foo
    {
        private readonly IDisposable bar;
        
        public Foo(IDisposable bar)
        {
            this.bar = bar;
        }
    }
}";
                AnalyzerAssert.Valid<IDISP002DisposeMember>(testCode);
            }

            [Test]
            public void IgnoreAssignedWithCtorArgumentIndexer()
            {
                var testCode = @"
namespace RoslynSandbox
{
    using System;

    public sealed class Foo
    {
        private readonly IDisposable bar;
        
        public Foo(IDisposable[] bars)
        {
            this.bar = bars[0];
        }
    }
}";
                AnalyzerAssert.Valid<IDISP002DisposeMember>(testCode);
            }

            [Test]
            public void IgnoreInjectedAndCreatedField()
            {
                var testCode = @"
namespace RoslynSandbox
{
    using System;

    public sealed class Foo
    {
        private readonly IDisposable bar;

        public Foo(IDisposable bar)
        {
            this.bar = bar;
        }

        public static Foo Create() => new Foo(new Disposable());
    }
}";
                AnalyzerAssert.Valid<IDISP002DisposeMember>(DisposableCode, testCode);
            }

            [Test]
            public void IgnoreInjectedAndCreatedProperty()
            {
                var testCode = @"
namespace RoslynSandbox
{
    using System;

    public sealed class Foo
    {
        public Foo(IDisposable bar)
        {
            this.Bar = bar;
        }

        public IDisposable Bar { get; }

        public static Foo Create() => new Foo(new Disposable());
    }
}";
                AnalyzerAssert.Valid<IDISP002DisposeMember>(DisposableCode, testCode);
            }

            [Test]
            public void IgnoreInjectedAndCreatedPropertyWhenFactoryTouchesIndexer()
            {
                var testCode = @"
namespace RoslynSandbox
{
    using System;

    public sealed class Foo
    {
        private readonly IDisposable bar;

        public Foo(IDisposable bar)
        {
            this.bar = bar;
        }

        public static Foo Create()
        {
            var disposables = new[] { new Disposable() };
            return new Foo(disposables[0]);
        }
    }
}";
                AnalyzerAssert.Valid<IDISP002DisposeMember>(DisposableCode, testCode);
            }

            [Test]
            public void IgnoreDictionaryPassedInViaCtor()
            {
                var testCode = @"
namespace RoslynSandbox
{
    using System.Collections.Concurrent;
    using System.IO;

    public class Foo
    {
        private readonly Stream current;

        public Foo(ConcurrentDictionary<int, Stream> streams)
        {
            this.current = streams[1];
        }
    }
}";
                AnalyzerAssert.Valid<IDISP002DisposeMember>(testCode);
            }

            [Test]
            public void IgnorePassedInViaCtorUnderscore()
            {
                var testCode = @"
namespace RoslynSandbox
{
    using System;

    public sealed class Foo
    {
        private readonly IDisposable _bar;
        
        public Foo(IDisposable bar)
        {
            _bar = bar;
        }
    }
}";
                AnalyzerAssert.Valid<IDISP002DisposeMember>(testCode);
            }

            [Test]
            public void IgnorePassedInViaCtorUnderscoreWhenClassIsDisposable()
            {
                var testCode = @"
namespace RoslynSandbox
{
    using System;

    public sealed class Foo : IDisposable
    {
        private readonly IDisposable _bar;
        
        public Foo(IDisposable bar)
        {
            _bar = bar;
        }

        public void Dispose()
        {
        }
    }
}";
                AnalyzerAssert.Valid<IDISP002DisposeMember>(testCode);
            }
        }
    }
}