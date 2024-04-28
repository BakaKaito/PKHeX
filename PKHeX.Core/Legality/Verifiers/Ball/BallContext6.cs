using System;
using static PKHeX.Core.Ball;
using static PKHeX.Core.Species;
using static PKHeX.Core.BallInheritanceResult;

namespace PKHeX.Core;

/// <summary>
/// Ball Inheritance Permissions for <see cref="EntityContext.Gen6"/> games.
/// </summary>
public sealed class BallContext6 : IBallContext
{
    public static readonly BallContext6 Instance = new();

    public bool CanBreedWithBall(ushort species, byte form, Ball ball)
    {
        // Eagerly return true for the most common case
        if (ball is Poke)
            return true;

        if (species >= Permit.Length)
            return false;
        var permitBit = GetPermitBit(ball);
        if (permitBit == BallType.None)
            return false;

        var permit = Permit[species];
        if ((permit & (1 << (byte)permitBit)) == 0)
            return false;
        return true;
    }

    public BallInheritanceResult CanBreedWithBall(ushort species, byte form, Ball ball, PKM pk)
    {
        // Eagerly return true for the most common case
        if (ball is Poke)
            return Valid;

        if (species >= Permit.Length)
            return Invalid;
        var permitBit = GetPermitBit(ball);
        if (permitBit == BallType.None)
            return Invalid;

        var permit = Permit[species];
        if ((permit & (1 << (byte)permitBit)) == 0)
            return Invalid;

        if (!BallContextHOME.IsAbilityPatchPossible(pk.Format, species) && !IsAbilityAllowed(species, form, pk, permitBit))
            return BadAbility;
        return Valid;
    }

    private static bool IsAbilityAllowed(ushort species, byte form, PKM pk, BallType permitBit) => permitBit switch
    {
        BallType.Gen3 => IsNotHidden(pk.AbilityNumber) || !IsBannedHiddenGen3(species, form),
        BallType.Gen4 => IsNotHidden(pk.AbilityNumber) || !IsBannedHiddenGen4(species, form),
        BallType.Safari => IsNotHidden(pk.AbilityNumber),
        BallType.Apricorn => IsNotHidden(pk.AbilityNumber),
        BallType.Sport => IsNotHidden(pk.AbilityNumber),
        BallType.Dream => IsNotHidden(pk.AbilityNumber) || !IsBannedHiddenDream(species),
        _ => true,
    };

    private static bool IsNotHidden(int pkAbilityNumber) => pkAbilityNumber != 4;

    private static bool IsBannedHiddenGen4(ushort species, byte form) => species switch
    {
        (int)Chikorita => true,
        (int)Cyndaquil => true,
        (int)Totodile => true,
        (int)Treecko => true,
        (int)Torchic => true,
        (int)Mudkip => true,
        (int)Turtwig => true,
        (int)Chimchar => true,
        (int)Piplup => true,
        (int)Snivy => true,
        (int)Tepig => true,
        (int)Oshawott => true,
        (int)Deerling => form != 0, // not Deerling-Spring

        // Fossil Only obtain
        (int)Archen => true,
        (int)Tyrunt => true,
        (int)Amaura => true,
        _ => false,
    };

    private static bool IsBannedHiddenGen3(ushort species, byte form) => species switch
    {
        // can have HA and can be in gen 3 ball as eggs but can not at same time.
        (int)Chikorita => true,
        (int)Cyndaquil => true,
        (int)Totodile => true,
        (int)Deerling => form != 0, // not Deerling-Spring
        (int)Pumpkaboo => form == 3, //Pumpkaboo-Super
        _ => false,
    };

    private static bool IsBannedHiddenDream(ushort species) => species switch
    {
        (int)Plusle => true,
        (int)Minun => true,
        (int)Kecleon => true,
        (int)Duskull => true,
        _ => false,
    };

    private static BallType GetPermitBit(Ball ball) => ball switch
    {
        Ultra => BallType.Gen3,
        Great => BallType.Gen3,
        Poke => BallType.Gen3,
        Safari => BallType.Safari,

        Net => BallType.Gen3,
        Dive => BallType.Gen3,
        Nest => BallType.Gen3,
        Repeat => BallType.Gen3,
        Timer => BallType.Gen3,
        Luxury => BallType.Gen3,
        Premier => BallType.Gen3,

        Dusk => BallType.Gen4,
        Heal => BallType.Gen4,
        Quick => BallType.Gen4,

        Fast => BallType.Apricorn,
        Level => BallType.Apricorn,
        Lure => BallType.Apricorn,
        Heavy => BallType.Apricorn,
        Love => BallType.Apricorn,
        Friend => BallType.Apricorn,
        Moon => BallType.Apricorn,

        Sport => BallType.Sport,
        Dream => BallType.Dream,
        _ => BallType.None,
    };

    public static ReadOnlySpan<byte> Permit =>
    [
        0x00, 0x03, 0x03, 0x03, 0x03, 0x03, 0x03, 0x03, 0x03, 0x03, 0x3B, 0x03, 0x03, 0x3B, 0x03, 0x03, 0x2F, 0x03, 0x03, 0x2F, // 000 - 019
        0x03, 0x2F, 0x03, 0x2F, 0x03, 0x03, 0x03, 0x2F, 0x03, 0x2F, 0x03, 0x03, 0x2F, 0x03, 0x03, 0x03, 0x03, 0x2B, 0x03, 0x03, // 020 - 039
        0x03, 0x2F, 0x03, 0x2F, 0x03, 0x03, 0x3F, 0x03, 0x3F, 0x03, 0x2F, 0x03, 0x2B, 0x03, 0x2F, 0x03, 0x2B, 0x03, 0x2B, 0x03, // 040 - 059
        0x2F, 0x03, 0x03, 0x2F, 0x03, 0x03, 0x2F, 0x03, 0x03, 0x2F, 0x03, 0x03, 0x2B, 0x03, 0x2F, 0x03, 0x03, 0x2F, 0x03, 0x2F, // 060 - 079
        0x03, 0x00, 0x00, 0x2F, 0x2F, 0x03, 0x2B, 0x03, 0x2F, 0x03, 0x2B, 0x03, 0x2F, 0x03, 0x03, 0x2F, 0x2F, 0x03, 0x2F, 0x03, // 080 - 099
        0x00, 0x00, 0x2F, 0x03, 0x2F, 0x03, 0x03, 0x03, 0x2F, 0x2F, 0x03, 0x2F, 0x03, 0x2F, 0x2F, 0x2F, 0x2B, 0x03, 0x2F, 0x03, // 100 - 119
        0x00, 0x00, 0x2F, 0x37, 0x03, 0x03, 0x03, 0x37, 0x00, 0x2F, 0x03, 0x2F, 0x00, 0x23, 0x03, 0x03, 0x03, 0x00, 0x23, 0x03, // 120 - 139
        0x23, 0x03, 0x23, 0x2B, 0x00, 0x00, 0x00, 0x2F, 0x03, 0x03, 0x00, 0x00, 0x81, 0x03, 0x03, 0x81, 0x03, 0x03, 0x81, 0x03, // 140 - 159
        0x03, 0x2F, 0x03, 0x2F, 0x03, 0x2F, 0x03, 0x2F, 0x03, 0x03, 0x2B, 0x03, 0x0F, 0x2F, 0x2F, 0x23, 0x03, 0x2F, 0x03, 0x2F, // 160 - 179
        0x03, 0x03, 0x03, 0x2F, 0x03, 0x2B, 0x03, 0x2F, 0x03, 0x07, 0x2F, 0x2F, 0x03, 0x2F, 0x2F, 0x03, 0x03, 0x03, 0x2F, 0x03, // 180 - 199
        0x2F, 0x00, 0x2F, 0x2F, 0x2F, 0x03, 0x2B, 0x2F, 0x03, 0x2F, 0x03, 0x2B, 0x03, 0x2F, 0x2F, 0x2B, 0x2F, 0x03, 0x2B, 0x03, // 200 - 219
        0x2B, 0x03, 0x2B, 0x2F, 0x03, 0x2B, 0x2B, 0x2B, 0x2F, 0x03, 0x03, 0x2F, 0x03, 0x00, 0x2F, 0x2F, 0x00, 0x03, 0x2B, 0x2F, // 220 - 239
        0x2F, 0x2F, 0x03, 0x00, 0x00, 0x00, 0x2F, 0x03, 0x03, 0x00, 0x00, 0x00, 0x00, 0x03, 0x03, 0x00, 0x03, 0x03, 0x00, 0x03, // 240 - 259
        0x03, 0x2B, 0x03, 0x2F, 0x07, 0x3B, 0x03, 0x03, 0x03, 0x03, 0x27, 0x03, 0x03, 0x2F, 0x03, 0x03, 0x2B, 0x03, 0x2B, 0x03, // 260 - 279
        0x2B, 0x03, 0x03, 0x27, 0x03, 0x2F, 0x03, 0x2F, 0x03, 0x03, 0x33, 0x03, 0x00, 0x2B, 0x03, 0x03, 0x2B, 0x03, 0x2F, 0x27, // 280 - 299
        0x23, 0x03, 0x2B, 0x2B, 0x27, 0x07, 0x03, 0x2F, 0x03, 0x27, 0x03, 0x2B, 0x2B, 0x37, 0x37, 0x27, 0x2F, 0x03, 0x27, 0x03, // 300 - 319
        0x23, 0x03, 0x2B, 0x03, 0x27, 0x2B, 0x03, 0x2F, 0x27, 0x03, 0x03, 0x27, 0x03, 0x2B, 0x03, 0x27, 0x27, 0x00, 0x00, 0x2F, // 320 - 339
        0x03, 0x27, 0x03, 0x00, 0x00, 0x23, 0x03, 0x23, 0x03, 0x23, 0x03, 0x23, 0x27, 0x27, 0x03, 0x27, 0x03, 0x27, 0x2F, 0x2B, // 340 - 359
        0x2F, 0x23, 0x03, 0x27, 0x03, 0x03, 0x2B, 0x03, 0x03, 0x2B, 0x2B, 0x27, 0x03, 0x03, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, // 360 - 379
        0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x03, 0x03, 0x00, 0x03, 0x03, 0x00, 0x03, 0x03, 0x2F, 0x03, 0x03, 0x2F, // 380 - 399
        0x03, 0x3B, 0x03, 0x2F, 0x03, 0x03, 0x2F, 0x03, 0x23, 0x03, 0x23, 0x03, 0x2B, 0x03, 0x03, 0x3B, 0x03, 0x27, 0x2F, 0x03, // 400 - 419
        0x2B, 0x03, 0x23, 0x03, 0x03, 0x23, 0x03, 0x2B, 0x03, 0x03, 0x03, 0x23, 0x03, 0x2F, 0x23, 0x03, 0x00, 0x00, 0x2B, 0x2F, // 420 - 439
        0x2F, 0x2B, 0x23, 0x27, 0x03, 0x03, 0x2B, 0x27, 0x03, 0x27, 0x03, 0x27, 0x03, 0x27, 0x03, 0x2F, 0x23, 0x03, 0x2B, 0x23, // 440 - 459
        0x03, 0x03, 0x00, 0x03, 0x03, 0x03, 0x03, 0x03, 0x03, 0x03, 0x03, 0x03, 0x03, 0x03, 0x00, 0x03, 0x03, 0x03, 0x03, 0x00, // 460 - 479
        0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x03, 0x03, 0x00, 0x03, // 480 - 499
        0x03, 0x00, 0x03, 0x03, 0x03, 0x03, 0x03, 0x03, 0x03, 0x03, 0x03, 0x03, 0x03, 0x03, 0x03, 0x03, 0x03, 0x23, 0x03, 0x23, // 500 - 519
        0x03, 0x03, 0x03, 0x03, 0x23, 0x03, 0x03, 0x03, 0x03, 0x23, 0x03, 0x23, 0x23, 0x03, 0x03, 0x23, 0x03, 0x03, 0x00, 0x00, // 520 - 539
        0x03, 0x03, 0x03, 0x23, 0x03, 0x03, 0x23, 0x03, 0x23, 0x03, 0x23, 0x23, 0x03, 0x03, 0x03, 0x03, 0x23, 0x23, 0x03, 0x23, // 540 - 559
        0x03, 0x23, 0x03, 0x03, 0x23, 0x03, 0x00, 0x03, 0x03, 0x03, 0x03, 0x03, 0x03, 0x03, 0x03, 0x03, 0x03, 0x23, 0x03, 0x03, // 560 - 579
        0x23, 0x03, 0x23, 0x03, 0x03, 0x83, 0x03, 0x23, 0x23, 0x03, 0x03, 0x03, 0x03, 0x03, 0x23, 0x23, 0x03, 0x03, 0x03, 0x00, // 580 - 599
        0x00, 0x00, 0x03, 0x03, 0x03, 0x23, 0x03, 0x03, 0x03, 0x03, 0x23, 0x03, 0x03, 0x03, 0x03, 0x00, 0x23, 0x03, 0x23, 0x03, // 600 - 619
        0x03, 0x23, 0x00, 0x00, 0x23, 0x03, 0x03, 0x00, 0x00, 0x03, 0x03, 0x23, 0x23, 0x03, 0x03, 0x03, 0x03, 0x03, 0x00, 0x00, // 620 - 639
        0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x03, 0x03, 0x03, 0x03, 0x03, 0x03, 0x03, 0x03, 0x03, 0x03, // 640 - 659
        0x03, 0x03, 0x03, 0x03, 0x03, 0x03, 0x03, 0x03, 0x03, 0x03, 0x03, 0x03, 0x03, 0x03, 0x03, 0x03, 0x03, 0x03, 0x03, 0x03, // 660 - 679
        0x03, 0x03, 0x03, 0x03, 0x03, 0x03, 0x03, 0x03, 0x03, 0x03, 0x03, 0x03, 0x03, 0x03, 0x03, 0x03, 0x00, 0x03, 0x00, 0x03, // 680 - 699
        0x03, 0x03, 0x03, 0x00, 0x03, 0x03, 0x03, 0x03, 0x03, 0x03, 0x83, 0x03, 0x03, 0x03, 0x03, 0x03,
    ];

    private enum BallType : byte
    {
        Gen3 = 0,
        Gen4 = 1,
        Safari = 2,
        Apricorn = 3,
        Sport = 4,
        Dream = 5,

        None = 9,
    }
}
