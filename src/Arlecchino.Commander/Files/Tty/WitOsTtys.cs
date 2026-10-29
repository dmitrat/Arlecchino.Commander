using System;
using System.Runtime.InteropServices;

namespace Arlecchino.Commander.Files.Tty;

/// <summary>
/// What WitOS takes, and how it is asked. Its library is musl answering Linux's calls by Linux's numbers, the pair and
/// the window among them, on every machine it runs on, so the words are Linux's; only the name of the system differs,
/// since .NET names it apart from Linux.
/// </summary>
public sealed class WitOsTtys : PosixTtys
{
    /// <summary>The one of these there is.</summary>
    public static WitOsTtys Instance { get; } = new();

    private WitOsTtys() { }

    /// <summary>
    /// The name .NET gives the system. It is a field rather than a literal, since the analyzers know only the systems
    /// .NET ships for.
    /// </summary>
    private static readonly string Name = "WITOS";

    /// <summary>Whether this is the machine.</summary>
    public static bool Here => OperatingSystem.IsOSPlatform(Name);

    /// <inheritdoc/>
    internal override int NotMine => 0x100;

    /// <inheritdoc/>
    internal override short OwnSession => 0x80;

    /// <inheritdoc/>
    internal override nuint AsksWindow => 0x5413;

    /// <inheritdoc/>
    internal override nuint TellsWindow => 0x5414;

    /// <inheritdoc/>
    internal override int LineModesAt => 12;

    /// <inheritdoc/>
    internal override int Sizing(int handle, nuint request, ref Posix.Window window) =>
        LibrarySizing(handle, request, ref window);

    /// <summary>
    /// The library's own call, reached as Linux's is, with the argument where every other argument goes.
    /// </summary>
    /// <param name="handle">The terminal.</param>
    /// <param name="request">Which of the two.</param>
    /// <param name="window">The size, read or written.</param>
    /// <returns>Nought when it worked.</returns>
    [DllImport(Posix.Library, EntryPoint = "ioctl", SetLastError = true)]
    private static extern int LibrarySizing(int handle, nuint request, ref Posix.Window window);
}
