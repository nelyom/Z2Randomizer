﻿using NLog;
using Z2Randomizer.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Z2Randomizer.Core;

namespace Z2Randomizer.Core.Overworld;

public class WestHyrule : World
{
    private readonly new Logger logger = LogManager.GetCurrentClassLogger();

    public Location start;
    public Location fairy;
    public Location bagu;
    public Location jump;
    public Location medCave;
    public Location trophyCave;
    public Location palace1;
    public Location palace2;
    public Location palace3;
    public Location jar;
    public Location heart1;
    public Location heart2;
    public Location lifeNorth;
    public Location lifeSouth;
    public Location shieldTown;
    public Location bridge1;
    public Location bridge2;
    public Location pbagCave;
    private int bridgeCount;

    private Dictionary<Location, Location> bridgeConn;
    private Dictionary<Location, Location> cityConn;
    private Dictionary<Location, Location> caveConn;
    private Dictionary<Location, Location> graveConn;

    private List<Location> lostWoods;

    private readonly SortedDictionary<int, Terrain> terrains = new SortedDictionary<int, Terrain>
        {
            { 0x462F, Terrain.PALACE},
            { 0x4630, Terrain.CAVE },
            { 0x4631, Terrain.FOREST},
            { 0x4632, Terrain.CAVE },
            { 0x4633, Terrain.FOREST },
            { 0x4634, Terrain.GRASS },
            { 0x4635, Terrain.FOREST },
            { 0x4636, Terrain.ROAD },
            { 0x4637, Terrain.SWAMP },
            { 0x4638, Terrain.GRAVE },
            { 0x4639, Terrain.CAVE },
            { 0x463A, Terrain.CAVE },
            { 0x463B, Terrain.CAVE },
            { 0x463C, Terrain.CAVE },
            { 0x463D, Terrain.CAVE },
            { 0x463E, Terrain.CAVE },
            { 0x463F, Terrain.CAVE },
            { 0x4640, Terrain.GRAVE },
            { 0x4641, Terrain.CAVE },
            { 0x4642, Terrain.BRIDGE },
            { 0x4643, Terrain.BRIDGE },
            { 0x4644, Terrain.BRIDGE },
            { 0x4645, Terrain.BRIDGE },
            { 0x4646, Terrain.FOREST },
            { 0x4647, Terrain.SWAMP },
            { 0x4648, Terrain.FOREST },
            { 0x4649, Terrain.FOREST },
            { 0x464A, Terrain.FOREST },
            { 0x464B, Terrain.FOREST },
            { 0x464C, Terrain.FOREST },
            { 0x464D, Terrain.ROAD },
            //{ 0x464E, terrain.desert },
            { 0x464F, Terrain.DESERT },
            //{ 0x4658, terrain.bridge },
            //{ 0x4659, terrain.cave },
            //{ 0x465A, terrain.cave },
            { 0x465B, Terrain.GRAVE },
            { 0x465C, Terrain.TOWN },
            { 0x465E, Terrain.TOWN },
            { 0x465F, Terrain.TOWN },
            { 0x4660, Terrain.TOWN },
            { 0x4661, Terrain.FOREST },
            { 0x4662, Terrain.TOWN },
            { 0x4663, Terrain.PALACE },
            { 0x4664, Terrain.PALACE },
            { 0x4665, Terrain.PALACE }
    };

    private const int MAP_ADDR = 0x7480;

    public WestHyrule(Hyrule hy)
        : base(hy)
    {
        LoadLocations(0x4639, 4, terrains, Continent.WEST);
        LoadLocations(0x4640, 2, terrains, Continent.WEST);

        LoadLocations(0x462F, 10, terrains, Continent.WEST);
        LoadLocations(0x463D, 3, terrains, Continent.WEST);
        LoadLocations(0x4642, 12, terrains, Continent.WEST);
        LoadLocations(0x464F, 1, terrains, Continent.WEST);
        LoadLocations(0x465B, 2, terrains, Continent.WEST);
        LoadLocations(0x465E, 8, terrains, Continent.WEST);
        start = GetLocationByMap(0x80, 0x00);
        //reachableAreas = new HashSet<string>();
        Location jumpCave = GetLocationByMap(9, 0);
        jumpCave.NeedJump = true;
        medCave = GetLocationByMap(0x0E, 0);
        Location heartCave = GetLocationByMap(0x10, 0);
        Location fairyCave = GetLocationByMap(0x12, 0);
        fairyCave.NeedFairy = true;
        jump = GetLocationByMap(0xC5, 4);
        bagu = GetLocationByMap(0x18, 4);
        fairy = GetLocationByMap(0xCB, 4);
        lifeNorth = GetLocationByMap(0xC8, 4);
        lifeSouth = GetLocationByMap(0x06, 4);
        lifeNorth.NeedBagu = true;
        lifeSouth.NeedBagu = true;
        trophyCave = GetLocationByMap(0xE1, 0);
        raft = GetLocationByMem(0x4658);
        palace1 = GetLocationByMem(0x4663);
        palace1.PalNum = 1;
        palace2 = GetLocationByMem(0x4664);
        palace2.PalNum = 2;
        palace3 = GetLocationByMem(0x4665);
        palace3.PalNum = 3;
        jar = GetLocationByMem(0x4632);
        heart1 = GetLocationByMem(0x463F);
        heart2 = GetLocationByMem(0x4634);
        shieldTown = GetLocationByMem(0x465C);
        pbagCave = GetLocationByMem(0x463D);


        Location parapaCave1 = GetLocationByMap(07, 0);
        Location parapaCave2 = GetLocationByMap(0xC7, 0);
        Location jumpCave2 = GetLocationByMap(0xCB, 0);
        Location fairyCave2 = GetLocationByMap(0xD3, 0);
        bridge1 = GetLocationByMap(0x04, 0);
        bridge2 = GetLocationByMap(0xC5, 0);

        if (hy.Props.SaneCaves)
        {
            fairyCave.TerrainType = Terrain.CAVE;
        }

        caveConn = new Dictionary<Location, Location>();
        bridgeConn = new Dictionary<Location, Location>();
        cityConn = new Dictionary<Location, Location>();
        graveConn = new Dictionary<Location, Location>();

        //connections.Add(hammerEnter, hammerExit);
        //connections.Add(hammerExit, hammerEnter);
        //caveConn.Add(hammerEnter, hammerExit);
        //caveConn.Add(hammerExit, hammerEnter);
        connections.Add(parapaCave1, parapaCave2);
        connections.Add(parapaCave2, parapaCave1);
        caveConn.Add(parapaCave1, parapaCave2);
        caveConn.Add(parapaCave2, parapaCave1);
        connections.Add(jumpCave, jumpCave2);
        connections.Add(jumpCave2, jumpCave);
        caveConn.Add(jumpCave, jumpCave2);
        caveConn.Add(jumpCave2, jumpCave);
        connections.Add(fairyCave, fairyCave2);
        connections.Add(fairyCave2, fairyCave);
        caveConn.Add(fairyCave2, fairyCave);
        graveConn.Add(fairyCave, fairyCave2);
        connections.Add(lifeNorth, lifeSouth);
        connections.Add(lifeSouth, lifeNorth);
        cityConn.Add(lifeSouth, lifeNorth);
        cityConn.Add(lifeNorth, lifeSouth);
        connections.Add(bridge1, bridge2);
        connections.Add(bridge2, bridge1);
        bridgeConn.Add(bridge1, bridge2);
        bridgeConn.Add(bridge2, bridge1);

        enemies = new List<int> { 0x03, 0x04, 0x05, 0x11, 0x12, 0x14, 0x15, 0x16, 0x17, 0x18, 0x19, 0x1A, 0x1B, 0x1C, 0x1F, 0x20 };
        flyingEnemies = new List<int> { 0x06, 0x07, 0x0A, 0x0D, 0x0E };
        generators = new List<int> { 0x0B, 0x0C, 0x0F, 0x1D };
        smallEnemies = new List<int> { 0x03, 0x04, 0x05, 0x11, 0x12, 0x1C, 0x1F };
        largeEnemies = new List<int> { 0x20, 0x14, 0x15, 0x16, 0x17, 0x18, 0x19, 0x1A, 0x1B };
        enemyAddr = 0x48B0;
        enemyPtr = 0x45B1;

        //34 29 39 48 35 58 30 53 40
        overworldMaps = new List<int>() { 0x22, 0x1D, 0x27, 0x30, 0x23, 0x3A, 0x1E, 0x35, 0x28 };
        MAP_ROWS = 75;
        MAP_COLS = 64;
        baseAddr = 0x462F;
        VANILLA_MAP_ADDR = 0x506C;

        walkableTerrains = new List<Terrain>() { Terrain.DESERT, Terrain.GRASS, Terrain.FOREST, Terrain.SWAMP, Terrain.GRAVE };
        randomTerrains = new List<Terrain> { Terrain.DESERT, Terrain.GRASS, Terrain.FOREST, Terrain.SWAMP, Terrain.GRAVE, Terrain.MOUNTAIN };
        if (hy.Props.HideLocs)
        {
            unimportantLocs.Add(GetLocationByMem(0x4631));
            unimportantLocs.Add(GetLocationByMem(0x4633));
            unimportantLocs.Add(GetLocationByMem(0x4635));
            unimportantLocs.Add(GetLocationByMem(0x4637));
            unimportantLocs.Add(GetLocationByMem(0x4638));
            unimportantLocs.Add(GetLocationByMem(0x4646));
            unimportantLocs.Add(GetLocationByMem(0x4647));
            unimportantLocs.Add(GetLocationByMem(0x4648));
            unimportantLocs.Add(GetLocationByMem(0x4649));
            unimportantLocs.Add(GetLocationByMem(0x464A));
            unimportantLocs.Add(GetLocationByMem(0x464B));
            unimportantLocs.Add(GetLocationByMem(0x464C));
            unimportantLocs.Add(GetLocationByMem(0x464D));
            unimportantLocs.Add(GetLocationByMem(0x464F));
            if(!hy.Props.HelpfulHints)
            {
                unimportantLocs.Add(GetLocationByMem(0x465B));
            }
        }
        if (hy.Props.WestBiome == Biome.ISLANDS)
        {
            this.biome = Biome.ISLANDS;
        }
        else if (hy.Props.WestBiome == Biome.CANYON || hy.Props.WestBiome == Biome.DRY_CANYON)
        {
            this.biome = Biome.CANYON;
        }
        else if (hy.Props.WestBiome == Biome.MOUNTAINOUS)
        {
            this.biome = Biome.MOUNTAINOUS;
        }
        else if(hy.Props.WestBiome == Biome.CALDERA)
        {
            this.biome = Biome.CALDERA;
        }
        else if(hy.Props.WestBiome == Biome.MOUNTAINOUS)
        {
            this.biome = Biome.MOUNTAINOUS;
        }
        else if (hy.Props.WestBiome == Biome.VANILLA)
        {
            this.biome = Biome.VANILLA;
        }
        else if (hy.Props.WestBiome == Biome.VANILLA_SHUFFLE)
        {
            this.biome = Biome.VANILLA_SHUFFLE;
        }
        else
        {
            this.biome = Biome.VANILLALIKE;
        }

        section = new SortedDictionary<Tuple<int, int>, string>{
            { Tuple.Create(0x34, 0x17), "north" },
        { Tuple.Create(0x20, 0x1D), "north" },
        { Tuple.Create(0x2A, 0x25), "north" },
        { Tuple.Create(0x3C, 0x10), "north" },
        { Tuple.Create(0x56, 0x14), "mid" },
        { Tuple.Create(0x40, 0x3E), "parapa" },
        { Tuple.Create(0x4D, 0x15), "mid" },
        { Tuple.Create(0x39, 0x3D), "parapa" },
        { Tuple.Create(0x47, 0x08), "mid" },
        { Tuple.Create(0x5C, 0x30), "grave" },
        { Tuple.Create(0x29, 0x30), "parapa" },
        { Tuple.Create(0x2E, 0x37), "north" },
        { Tuple.Create(0x3A, 0x01), "north" },
        { Tuple.Create(0x3E, 0x03), "mid" },
        { Tuple.Create(0x3E, 0x26), "mid" },
        { Tuple.Create(0x45, 0x09), "hammer0" },
        { Tuple.Create(0x3E, 0x36), "hammer" },
        { Tuple.Create(0x60, 0x32), "grave" },
        { Tuple.Create(0x66, 0x3B), "island" },
        { Tuple.Create(0x52, 0x10), "mid" },
        { Tuple.Create(0x57, 0x1A), "mid" },
        { Tuple.Create(0x61, 0x1A), "dmexit" },
        { Tuple.Create(0x61, 0x22), "grave" },
        { Tuple.Create(0x40, 0x07), "mid" },
        { Tuple.Create(0x43, 0x11), "mid" },
        { Tuple.Create(0x57, 0x21), "mid" },
        { Tuple.Create(0x4C, 0x14), "mid" },
        { Tuple.Create(0x4D, 0x11), "mid" },
        { Tuple.Create(0x4E, 0x13), "mid" },
        { Tuple.Create(0x4D, 0x17), "mid" },
        { Tuple.Create(0x44, 0x25), "mid" },
        { Tuple.Create(0x66, 0x26), "grave" },
        { Tuple.Create(0x4D, 0x3D), "grave" },
        { Tuple.Create(0x5F, 0x0A), "lifesouth" },
        { Tuple.Create(0x60, 0x15), "dmexit" },
        { Tuple.Create(0x58, 0x32), "grave" },
        { Tuple.Create(0x36, 0x2E), "north" },
        { Tuple.Create(0x24, 0x02), "north" },
        { Tuple.Create(0x5B, 0x08), "lifesouth" },
        { Tuple.Create(0x59, 0x08), "mid" },
        { Tuple.Create(0x4C, 0x15), "mid" },
        { Tuple.Create(0x4B, 0x3C), "grave" },
        { Tuple.Create(0x20, 0x3E), "parapa" },
        { Tuple.Create(0x40, 0x0B), "mid" },
        { Tuple.Create(0x62, 0x39), "island" }
        };
        lostWoods = new List<Location> { GetLocationByMem(0x4649), GetLocationByMem(0x464A), GetLocationByMem(0x464B), GetLocationByMem(0x464C), GetLocationByMem(0x4635) };
    }

    public bool Terraform()
    {
        foreach (Location location in AllLocations)
        {
            location.CanShuffle = true;
        }
        if (this.biome == Biome.VANILLA || this.biome == Biome.VANILLA_SHUFFLE)
        {
            MAP_ROWS = 75;
            MAP_COLS = 64;
            ReadVanillaMap();
            if(this.biome == Biome.VANILLA_SHUFFLE)
            {
                areasByLocation = new SortedDictionary<string, List<Location>>
                {
                    { "north", new List<Location>() },
                    { "mid", new List<Location>() },
                    { "parapa", new List<Location>() },
                    { "grave", new List<Location>() },
                    { "lifesouth", new List<Location>() },
                    { "island", new List<Location>() },
                    { "hammer", new List<Location>() },
                    { "hammer0", new List<Location>() },
                    { "dmexit", new List<Location>() }
                };
                foreach (Location location in AllLocations)
                {
                    areasByLocation[section[location.Coords]].Add(GetLocationByCoords(location.Coords));
                }
                ChooseConn("parapa", connections, true);
                ChooseConn("lifesouth", connections, true);
                ChooseConn("island", connections, true);
                ChooseConn("dmexit", connections, true);

                ShuffleLocations(AllLocations);
                if (hyrule.Props.VanillaShuffleUsesActualTerrain)
                {
                    foreach (Location location in AllLocations)
                    {
                        map[location.Ypos - 30, location.Xpos] = location.TerrainType;
                    }
                }
                foreach(Location location in Locations[Terrain.CAVE])
                {
                    location.PassThrough = 0;
                }
                foreach (Location location in Locations[Terrain.TOWN])
                {
                    location.PassThrough = 0;
                }
                foreach (Location location in Locations[Terrain.PALACE])
                {
                    location.PassThrough = 0;
                }
                raft.PassThrough = 0;
                bridge1.PassThrough = 0;
                bridge2.PassThrough = 0;
                GetLocationByMap(0x12, 0).PassThrough = 0; //fairy cave

            }
        }
        else
        {
            Terrain fillerWater = hyrule.Props.CanWalkOnWaterWithBoots ? Terrain.WALKABLEWATER : Terrain.WATER;

            bytesWritten = 2000;

            if(hyrule.Props.BagusWoods)
            {
                bagu.CanShuffle = false;
                foreach(Location location in lostWoods)
                {
                    location.CanShuffle = false;
                    unimportantLocs.Remove(location);
                }
            }
            while (bytesWritten > MAP_SIZE_BYTES)
            {
                Terrain riverTerrain = Terrain.MOUNTAIN;
                lifeSouth.CanShuffle = false;
                lifeNorth.CanShuffle = false;

                map = new Terrain[MAP_ROWS, MAP_COLS];

                //blank the whole map to start
                for (int i = 0; i < MAP_ROWS; i++)
                {
                    for (int j = 0; j < MAP_COLS; j++)
                    {
                        map[i, j] = Terrain.NONE;
                    }
                }

                //Biome specifics
                switch(biome)
                {
                    case Biome.ISLANDS:
                        riverTerrain = fillerWater;

                        //Fill the edges with water
                        for (int i = 0; i < MAP_COLS; i++)
                        {
                            map[0, i] = fillerWater;
                            map[MAP_ROWS - 1, i] = fillerWater;
                        }
                        for (int i = 0; i < MAP_ROWS; i++)
                        {
                            map[i, 0] = fillerWater;
                            map[i, MAP_COLS - 1] = fillerWater;
                        }


                        //Stripe 1-3 rows/columns with water to establish rough island borders
                        int cols = hyrule.RNG.Next(2, 4);
                        int rows = hyrule.RNG.Next(2, 4);
                        List<int> pickedC = new List<int>();
                        List<int> pickedR = new List<int>();

                        while (cols > 0)
                        {
                            int col = hyrule.RNG.Next(10, MAP_COLS - 11);
                            if (!pickedC.Contains(col))
                            {
                                for (int i = 0; i < MAP_ROWS; i++)
                                {
                                    if (map[i, col] == Terrain.NONE)
                                    {
                                        map[i, col] = fillerWater;
                                    }
                                }
                                pickedC.Add(col);
                                cols--;
                            }
                        }
                        while (rows > 0)
                        {
                            int row = hyrule.RNG.Next(10, MAP_ROWS - 11);
                            if (!pickedR.Contains(row))
                            {
                                for (int i = 0; i < MAP_COLS; i++)
                                {
                                    if (map[row, i] == Terrain.NONE)
                                    {
                                        map[row, i] = fillerWater;
                                    }
                                }
                                pickedR.Add(row);
                                rows--;
                            }
                        }
                        lifeSouth.CanShuffle = false;
                        lifeNorth.CanShuffle = false;
                        walkableTerrains = new List<Terrain>() { Terrain.DESERT, Terrain.GRASS, Terrain.FOREST, Terrain.SWAMP, Terrain.GRAVE };
                        randomTerrains = new List<Terrain> { Terrain.DESERT, Terrain.GRASS, Terrain.FOREST, Terrain.SWAMP, Terrain.GRAVE, Terrain.MOUNTAIN, fillerWater };
                        break;

                    case Biome.CANYON:
                        isHorizontal = hyrule.RNG.NextDouble() > .5;
                        riverTerrain = fillerWater;
                        if (hyrule.Props.WestBiome == Biome.DRY_CANYON)
                        {
                            riverTerrain = Terrain.DESERT;
                            bridge1.CanShuffle = false;
                            bridge1.Ypos = 0;
                            bridge2.CanShuffle = false;
                            bridge2.Ypos = 0;
                        }
                        walkableTerrains = new List<Terrain>() { Terrain.DESERT, Terrain.GRASS, Terrain.FOREST,  Terrain.GRAVE, Terrain.MOUNTAIN };
                        randomTerrains = new List<Terrain> { Terrain.DESERT, Terrain.GRASS, Terrain.FOREST,  Terrain.GRAVE, Terrain.MOUNTAIN, fillerWater };


                        DrawCanyon(riverTerrain);
                        this.walkableTerrains.Remove(Terrain.MOUNTAIN);
                        //this.randomTerrains.Add(terrain.lava);
                        break;
                    case Biome.CALDERA:
                        this.isHorizontal = hyrule.RNG.NextDouble() > .5;
                        DrawCenterMountain();
                        palace3.CanShuffle = false;
                        walkableTerrains = new List<Terrain>() { Terrain.DESERT, Terrain.GRASS, Terrain.FOREST, Terrain.SWAMP, Terrain.GRAVE };
                        randomTerrains = new List<Terrain> { Terrain.DESERT, Terrain.GRASS, Terrain.FOREST, Terrain.SWAMP, Terrain.GRAVE, Terrain.MOUNTAIN, fillerWater };
                        break;

                    case Biome.MOUNTAINOUS:
                        walkableTerrains = new List<Terrain>() { Terrain.DESERT, Terrain.GRASS, Terrain.FOREST, Terrain.SWAMP, Terrain.GRAVE };
                        randomTerrains = new List<Terrain> { Terrain.DESERT, Terrain.GRASS, Terrain.FOREST, Terrain.SWAMP, Terrain.GRAVE, Terrain.MOUNTAIN, fillerWater };

                        riverTerrain = Terrain.MOUNTAIN;
                        for (int i = 0; i < MAP_COLS; i++)
                        {
                            map[0, i] = Terrain.MOUNTAIN;
                            map[MAP_ROWS - 1, i] = Terrain.MOUNTAIN;
                        }

                        for (int i = 0; i < MAP_ROWS; i++)
                        {
                            map[i, 0] = Terrain.MOUNTAIN;
                            map[i, MAP_COLS - 1] = Terrain.MOUNTAIN;
                        }


                        cols = hyrule.RNG.Next(2, 4);
                        rows = hyrule.RNG.Next(2, 4);
                        pickedC = new List<int>();
                        pickedR = new List<int>();

                        while (cols > 0)
                        {
                            int col = hyrule.RNG.Next(10, MAP_COLS - 11);
                            if (!pickedC.Contains(col))
                            {
                                for (int i = 0; i < MAP_ROWS; i++)
                                {
                                    if (map[i, col] == Terrain.NONE)
                                    {
                                        map[i, col] = Terrain.MOUNTAIN;
                                    }
                                }
                                pickedC.Add(col);
                                cols--;
                            }
                        }

                        while (rows > 0)
                        {
                            int row = hyrule.RNG.Next(10, MAP_ROWS - 11);
                            if (!pickedR.Contains(row))
                            {
                                for (int i = 0; i < MAP_COLS; i++)
                                {
                                    if (map[row, i] == Terrain.NONE)
                                    {
                                        map[row, i] = Terrain.MOUNTAIN;
                                    }
                                }
                                pickedR.Add(row);
                                rows--;
                            }
                        }
                        lifeSouth.CanShuffle = false;
                        lifeNorth.CanShuffle = false;
                        break;
                    default:
                        walkableTerrains = new List<Terrain>() { Terrain.DESERT, Terrain.GRASS, Terrain.FOREST, Terrain.SWAMP, Terrain.GRAVE };
                        randomTerrains = new List<Terrain> { Terrain.DESERT, Terrain.GRASS, Terrain.FOREST, Terrain.SWAMP, Terrain.GRAVE, Terrain.MOUNTAIN, fillerWater };
                        //drawRoad();
                        DrawMountains();
                        //drawBridge();
                        DrawRiver(new List<Location>() { GetLocationByMem(0x4642), GetLocationByMem(0x4643) });
                        break;
                }

                Direction raftDirection = Direction.EAST;
                if (hyrule.Props.ContinentConnections != ContinentConnectionType.NORMAL && this.biome != Biome.CANYON)
                {
                    raftDirection = (Direction)hyrule.RNG.Next(4);
                }
                else if (this.biome == Biome.CANYON)
                {
                    raftDirection = isHorizontal ? DirectionExtensions.RandomHorizontal(hyrule.RNG) : DirectionExtensions.RandomVertical(hyrule.RNG);
                }
                if (raft != null)
                {
                    DrawOcean(raftDirection);
                }


                Direction bridgeDirection = (Direction)hyrule.RNG.Next(4);
                do
                {
                    if (this.biome != Biome.CANYON && this.biome != Biome.CALDERA)
                    {
                        bridgeDirection = (Direction)hyrule.RNG.Next(4);
                    }
                    else
                    {
                        bridgeDirection = isHorizontal ? DirectionExtensions.RandomHorizontal(hyrule.RNG) : DirectionExtensions.RandomVertical(hyrule.RNG);
                    }
                } while (bridgeDirection == raftDirection);
                if (bridge != null)
                {
                    DrawOcean(bridgeDirection);
                }
                bool b = PlaceLocations(riverTerrain);
                if (!b)
                {
                    failedOnPlaceLocations++;
                    return false;
                }

                if (hyrule.Props.BagusWoods)
                {
                    bool f = PlaceBagu();
                    if (!f)
                    {
                        failedOnBaguPlacement++;
                        return false;
                    }
                }

                if (hyrule.Props.HideLocs)
                {
                    PlaceRandomTerrain(50);
                }
                else
                {
                    PlaceRandomTerrain(15);
                }


                if (!GrowTerrain())
                {
                    return false;
                }


                if (raft != null)
                {
                    if (!DrawRaft(false, raftDirection))
                    {
                        failedOnRaftPlacement++;
                        return false;
                    }
                }

                if (bridge != null)
                {
                    if (!DrawRaft(true, bridgeDirection))
                    {
                        failedOnBridgePlacement++;
                        return false;
                    }
                }
                
                if (this.biome == Biome.CALDERA)
                {

                    bool f = ConnectIslands(1, true, fillerWater, false, false, false);
                    if (!f)
                    {
                        failedOnIslandConnection++;
                        return false;
                    }

                    bool g = MakeCaldera();
                    if (!g)
                    {
                        failedOnMakeCaldera++;
                        return false;
                    }
                }
                PlaceRocks();
                
                PlaceHiddenLocations();

                int bridges = 10;

                if (this.biome == Biome.CANYON)
                {
                    bridges = 100;
                    bool f = ConnectIslands(bridges, true, riverTerrain, false, true, false);
                    if (!f)
                    {
                        failedOnConnectIslands++;
                        return false;
                    }
                }
                if (this.biome == Biome.ISLANDS)
                {
                    bridges = 25;
                    bool f = ConnectIslands(bridges, true, riverTerrain, false, true, false);
                    if (!f)
                    {
                        failedOnConnectIslands++;
                        return false;
                    }
                }
                if (this.biome == Biome.MOUNTAINOUS)
                {
                    bridges = 15;
                    this.walkableTerrains.Add(Terrain.ROAD);

                    bool h = ConnectIslands(bridges, true, riverTerrain, false, false, false);
                    if (!h)
                    {
                        failedOnConnectIslands++;
                        return false;
                    }
                }
                if (this.biome == Biome.VANILLALIKE)
                {
                    bridges = 4;
                    riverTerrain = fillerWater;
                    ConnectIslands(2, false, Terrain.MOUNTAIN, false, false, false);
                    bool f = ConnectIslands(bridges, true, riverTerrain, false, true, false);
                    if (!f)
                    {
                        failedOnConnectIslands++;
                        return false;
                    }
                }

                foreach (Location location in Locations[Terrain.ROAD])
                {
                    if (location.CanShuffle)
                    {
                        location.Ypos = 0;
                        location.CanShuffle = false;
                    }
                }

                foreach (Location location in Locations[Terrain.BRIDGE])
                {
                    if (location.CanShuffle)
                    {
                        location.Ypos = 0;
                        location.CanShuffle = false;
                    }
                }


                //check bytes and adjust
                WriteMapToRom(false, MAP_ADDR, MAP_SIZE_BYTES, 0, 0);
                logger.Debug("West:" + bytesWritten);
            }
        }
        WriteMapToRom(true, MAP_ADDR, MAP_SIZE_BYTES, 0, 0);

        visitation = new bool[MAP_ROWS, MAP_COLS];
        for (int i = 0; i < MAP_ROWS; i++)
        {
            for (int j = 0; j < MAP_COLS; j++)
            {
                visitation[i, j] = false;
            }
        }

        visitation[start.Ypos - 30, start.Xpos] = true;
        return true;
    }

    public void SetStart()
    {
        visitation[start.Ypos - 30, start.Xpos] = true;
        start.Reachable = true;
    }

    private bool PlaceBagu()
    {
        int y = hyrule.RNG.Next(6, MAP_ROWS - 7);
        int x = hyrule.RNG.Next(6, MAP_COLS - 7);
        int tries = 0;
        while((map[y, x] != Terrain.NONE || GetLocationByCoords(Tuple.Create(y + 30, x)) != null) && tries < 1000)
        {
            y = hyrule.RNG.Next(6, MAP_ROWS - 7);
            x = hyrule.RNG.Next(6, MAP_COLS - 7);
        }
        if(tries >= 1000)
        {
            return false;
        }
        bagu.Ypos = y + 30;
        bagu.Xpos = x;
        map[y, x] = Terrain.FOREST;

        int placed = 0;
        tries = 0;
        while(placed < 5 && tries < 3000)
        {
            int newx = hyrule.RNG.Next(x - 3, x + 4);
            int newy = hyrule.RNG.Next(y - 3, y + 4);
            while((map[newy, newx] != Terrain.NONE || GetLocationByCoords(Tuple.Create(newy + 30, newx)) != null) && tries < 100)
            {
                newx = hyrule.RNG.Next(x - 3, x + 4);
                newy = hyrule.RNG.Next(y - 3, y + 4);
                tries++;
            }
            lostWoods[placed].Ypos = newy + 30;
            lostWoods[placed].Xpos = newx;
            map[newy, newx] = Terrain.FOREST;
            placed++;
        }
        if(tries >= 3000 && placed < 3)
        {
            return false;
        }
        else
        {
            for(int i = placed; i < lostWoods.Count; i++)
            {
                lostWoods[placed].Ypos = 0;
            }
        }
        return true;
    }
    private bool MakeCaldera()
    {
        Terrain water = Terrain.WATER;
        if(hyrule.Props.CanWalkOnWaterWithBoots)
        {
            water = Terrain.WALKABLEWATER;
        }
        int centerx = hyrule.RNG.Next(21, 41);
        int centery = hyrule.RNG.Next(32, 42);
        if (isHorizontal)
        {
            centerx = hyrule.RNG.Next(27, 37);
            centery = hyrule.RNG.Next(22, 52);
        }

        bool placeable = false;
        do
        {
            if (isHorizontal)
            {
                centerx = hyrule.RNG.Next(27, 37);
                centery = hyrule.RNG.Next(22, 52);
            }
            else
            {
                centerx = hyrule.RNG.Next(21, 41);
                centery = hyrule.RNG.Next(32, 42);
            }
            placeable = true;
            for (int i = centery - 7; i < centery + 8; i++)
            {
                for (int j = centerx - 7; j < centerx + 8; j++)
                {
                    if (map[i, j] != Terrain.MOUNTAIN)
                    {
                        placeable = false;
                    }
                }
            }
        } while (!placeable);

        int startx = centerx - 5;
        int starty = centery;
        int deltax = 1;
        int deltay = 0;
        if (!isHorizontal)
        {
            startx = centerx;
            starty = centery - 5;
            deltax = 0;
            deltay = 1;
        }
        for(int i = 0; i < 10; i++)
        {
            int lake = hyrule.RNG.Next(7, 11);
            if(i == 0 || i == 9)
            {
                lake = hyrule.RNG.Next(3, 6);
            }
            if (isHorizontal)
            {
                for(int j = 0; j < lake / 2; j++)
                {
                    map[starty + j, startx] = water;
                    if(i == 0)
                    {
                        map[starty + j, startx - 1] = Terrain.FOREST;
                    }
                    if(i == 9)
                    {
                        map[starty + j, startx + 1] = Terrain.FOREST;
                    }
                    
                }
                int top = starty + lake / 2;
                while(map[top, startx - 1] == Terrain.MOUNTAIN)
                {
                    map[top, startx - 1] = Terrain.FOREST;
                    top--;
                }
                top = starty + lake / 2;
                while (map[top, startx - 1] != Terrain.MOUNTAIN)
                {
                    map[top, startx] = Terrain.FOREST;
                    top++;
                }

                for (int j = 0; j < lake - (lake / 2); j++)
                {
                    map[starty - j, startx] = water;
                    if (i == 0)
                    {
                        map[starty - j, startx - 1] = Terrain.FOREST;
                    }
                    if (i == 9)
                    {
                        map[starty - j, startx + 1] = Terrain.FOREST;
                    }
                    
                }
                top = starty - (lake - (lake / 2));
                while (map[top, startx - 1] == Terrain.MOUNTAIN)
                {
                    map[top, startx - 1] = Terrain.FOREST;
                    top++;
                }
                top = starty - (lake - (lake / 2));
                while (map[top, startx - 1] != Terrain.MOUNTAIN)
                {
                    map[top, startx] = Terrain.FOREST;
                    top--;
                }

                //map[starty + lake / 2, startx] = terrain.forest;
               // map[starty - (lake - (lake / 2)), startx] = terrain.forest;
                if (i == 0)
                {
                    map[starty + lake / 2, startx + 1] = Terrain.FOREST;
                    map[starty - (lake - (lake / 2)), startx - 1] = Terrain.FOREST;
                }
                if (i == 9)
                {
                    map[starty + lake / 2, startx + 1] = Terrain.FOREST;
                    map[starty - (lake - (lake / 2) ), startx + 1] = Terrain.FOREST;
                }

            }
            else
            {
                for (int j = 0; j < lake / 2; j++)
                {
                    map[starty, startx + j] = water;
                    if (i == 0)
                    {
                        map[starty - 1, startx + j] = Terrain.FOREST;
                    }
                    if (i == 9)
                    {
                        map[starty + 1, startx + j] = Terrain.FOREST;
                    }
                }
                int top = startx + lake / 2;
                while (map[starty - 1, top] == Terrain.MOUNTAIN && i != 0)
                {
                    map[starty - 1, top] = Terrain.FOREST;
                    top--;
                }
                top = startx + lake / 2;
                while (map[starty - 1, top] != Terrain.MOUNTAIN && i != 0)
                {
                    map[starty, top] = Terrain.FOREST;
                    top++;
                }

                for (int j = 0; j < lake - (lake / 2); j++)
                {
                    map[starty, startx - j] = water;
                    if (i == 0)
                    {
                        map[starty - 1, startx - j] = Terrain.FOREST;
                    }
                    if (i == 9)
                    {
                        map[starty + 1, startx - j] = Terrain.FOREST;
                    }
                }
                 top = startx - (lake - (lake / 2));
                while (map[starty - 1, top] == Terrain.MOUNTAIN && i != 0)
                {
                    map[starty - 1, top] = Terrain.FOREST;
                    top++;
                }
                top = startx - (lake - (lake / 2));
                while (map[starty - 1, top] != Terrain.MOUNTAIN && i != 0)
                {
                    map[starty, top] = Terrain.FOREST;
                    top--;
                }
                //map[starty, startx + lake / 2] = terrain.forest;
                //map[starty, startx - (lake - (lake / 2))] = terrain.forest;
                if (i == 0)
                {
                    map[starty - 1, startx + lake / 2] = Terrain.FOREST;
                    map[starty - 1, startx - (lake - (lake / 2))] = Terrain.FOREST;
                }
                if (i == 9)
                {
                    map[starty + 1, startx + lake / 2] = Terrain.FOREST;
                    map[starty + 1, startx - (lake - (lake / 2))] = Terrain.FOREST;
                }
            }
            startx += deltax;
            starty += deltay;
        }
        int caves = hyrule.RNG.Next(2) + 1;
        Location cave1l = new Location();
        Location cave1r = new Location();
        Location cave2l = new Location();
        Location cave2r = new Location();
        int numCaves = 2;
        if(hyrule.Props.SaneCaves)
        {
            numCaves++;
        }
        int cavenum1 = hyrule.RNG.Next(numCaves);
        if(cavenum1 == 0)
        {
            cave1l = GetLocationByMap(9, 0);//jump cave
            cave1r = GetLocationByMap(0xCB, 0);
        }
        else if (cavenum1 == 1)
        {
            cave1l = GetLocationByMap(07, 0); //parappa
            cave1r = GetLocationByMap(0xC7, 0);
        }
        else
        {
            cave1l = GetLocationByMap(0x12, 0); //fairy cave
            cave1r = GetLocationByMap(0xD3, 0);
        }
        map[cave1l.Ypos - 30, cave1l.Xpos] = Terrain.MOUNTAIN;
        map[cave1r.Ypos - 30, cave1r.Xpos] = Terrain.MOUNTAIN;
        if (caves > 1)
        {
            int cavenum2 = hyrule.RNG.Next(numCaves);
            while(cavenum2 == cavenum1)
            {
                cavenum2 = hyrule.RNG.Next(numCaves);
            }
            if (cavenum2 == 0)
            {
                cave2l = GetLocationByMap(9, 0);//jump cave
                cave2r = GetLocationByMap(0xCB, 0);
            }
            else if (cavenum2 == 1)
            {
                cave2l = GetLocationByMap(07, 0); //parappa
                cave2r = GetLocationByMap(0xC7, 0);
            }
            else
            {
                cave2l = GetLocationByMap(0x12, 0); //fairy cave
                cave2r = GetLocationByMap(0xD3, 0);
            }
            map[cave2l.Ypos - 30, cave2l.Xpos] = Terrain.MOUNTAIN;
            map[cave2r.Ypos - 30, cave2r.Xpos] = Terrain.MOUNTAIN;
        }
        //
        int caveType = hyrule.RNG.Next(2);
        if (isHorizontal)
        {
            bool f = HorizontalCave(caveType, centerx, centery, cave1l, cave1r);
            if(!f)
            {
                return false;
            }

            if(caves > 1)
            {
                if(caveType == 0)
                {
                    caveType = 1;
                }
                else
                {
                    caveType = 0;
                }
                f = HorizontalCave(caveType, centerx, centery, cave2l, cave2r);
                if (!f)
                {
                    return false;
                }
            }
            
            if(caves == 1)
            {
                int delta = -1;
                if(caveType == 0) //palace goes right
                {
                    delta = 1;
                }
                int palacex = centerx;
                int palacey = hyrule.RNG.Next(centery - 2, centery + 3);
                while (map[palacey, palacex] != Terrain.MOUNTAIN)
                {
                    palacex += delta;
                }
                map[palacey, palacex] = Terrain.PALACE;
                palace3.Ypos = palacey + 30;
                palace3.Xpos = palacex;
                map[palacey, palacex + delta] = Terrain.MOUNTAIN;

            }
            else
            {
                int palaceType = hyrule.RNG.Next(2);
                int delta = -1;
                if(palaceType == 0)
                {
                    delta = 1;
                }
                int palacex = hyrule.RNG.Next(centerx - 2, centerx + 3);
                int palacey = centery;
                while (map[palacey, palacex] != Terrain.MOUNTAIN)
                {
                    palacey += delta;
                }
                map[palacey, palacex] = Terrain.PALACE;
                palace3.Ypos = palacey + 30;
                palace3.Xpos = palacex;
                map[palacey + delta, palacex] = Terrain.MOUNTAIN;

            }

        }
        else //Vertical
        {
            bool f = VerticalCave(caveType, centerx, centery, cave1l, cave1r);
            if (!f)
            {
                return false;
            }

            if (caves > 1)
            {
                if (caveType == 0)
                {
                    caveType = 1;
                }
                else
                {
                    caveType = 0;
                }
                f = VerticalCave(caveType, centerx, centery, cave2l, cave2r);
                if (!f)
                {
                    return false;
                }
            }

            if (caves == 1)
            {
                int delta = -1;
                if (caveType == 0) //palace goes down
                {
                    delta = 1;
                }
                int palacex = hyrule.RNG.Next(centerx - 2, centerx + 3);
                int palacey = centery;
                while (map[palacey, palacex] != Terrain.MOUNTAIN)
                {
                    palacey += delta;
                }
                map[palacey, palacex] = Terrain.PALACE;
                palace3.Ypos = palacey + 30;
                palace3.Xpos = palacex;
                map[palacey + delta, palacex] = Terrain.MOUNTAIN;


            }
            else
            {
                int palaceType = hyrule.RNG.Next(2);
                int delta = -1;
                if (palaceType == 0)
                {
                    delta = 1;
                }
                int palacex = centerx;
                int palacey = hyrule.RNG.Next(centery - 2, centery + 3);
                while (map[palacey, palacex] != Terrain.MOUNTAIN)
                {
                    palacex += delta;
                }
                map[palacey, palacex] = Terrain.PALACE;
                palace3.Ypos = palacey + 30;
                palace3.Xpos = palacex;
                map[palacey, palacex + delta] = Terrain.MOUNTAIN;

            }
        }
        return true;
    }
    private void PlaceRocks()
    {
        int rockNum = hyrule.RNG.Next(3);
        int cavePicked = 0;
        while (rockNum > 0)
        {
            List<Location> Caves = Locations[Terrain.CAVE];
            Location cave = Caves[hyrule.RNG.Next(Caves.Count)];
            int caveConn = 0;
            if(caveConn != 0 && connections.ContainsKey(GetLocationByMem(cavePicked)))
            {
                caveConn = connections[GetLocationByMem(cavePicked)].MemAddress;
            }
            if (hyrule.Props.BoulderBlockConnections && cave.MemAddress != cavePicked && cave.MemAddress != caveConn)
            {
                if (map[cave.Ypos - 30, cave.Xpos - 1] != Terrain.MOUNTAIN && cave.Xpos + 2 < MAP_COLS && GetLocationByCoords(Tuple.Create(cave.Ypos - 30, cave.Xpos + 2)) == null)
                {
                    map[cave.Ypos - 30, cave.Xpos - 1] = Terrain.ROCK;
                    map[cave.Ypos - 30, cave.Xpos] = Terrain.ROAD;
                    map[cave.Ypos - 30, cave.Xpos + 1] = Terrain.CAVE;
                    if (cave.Xpos + 2 < MAP_COLS)
                    {
                        map[cave.Ypos - 30, cave.Xpos + 2] = Terrain.MOUNTAIN;
                    }
                    cave.Xpos++;
                    rockNum--;
                }
                else if (map[cave.Ypos - 30, cave.Xpos + 1] != Terrain.MOUNTAIN && cave.Xpos - 2 > 0 && GetLocationByCoords(Tuple.Create(cave.Ypos - 30, cave.Xpos - 2)) == null)
                {
                    map[cave.Ypos - 30, cave.Xpos + 1] = Terrain.ROCK;
                    map[cave.Ypos - 30, cave.Xpos] = Terrain.ROAD;
                    map[cave.Ypos - 30, cave.Xpos - 1] = Terrain.CAVE;
                    if (cave.Xpos - 2 >= 0)
                    {
                        map[cave.Ypos - 30, cave.Xpos - 2] = Terrain.MOUNTAIN;
                    }
                    cave.Xpos--;
                    rockNum--;
                }
                else if (map[cave.Ypos - 29, cave.Xpos] != Terrain.MOUNTAIN && cave.Ypos - 32 < MAP_COLS && GetLocationByCoords(Tuple.Create(cave.Ypos - 32, cave.Xpos)) == null)
                {
                    map[cave.Ypos - 29, cave.Xpos] = Terrain.ROCK;
                    map[cave.Ypos - 30, cave.Xpos] = Terrain.ROAD;
                    map[cave.Ypos - 31, cave.Xpos] = Terrain.CAVE;
                    if (cave.Ypos - 32 >= 0)
                    {
                        map[cave.Ypos - 32, cave.Xpos] = Terrain.MOUNTAIN;
                    }
                    cave.Ypos--;
                    rockNum--;
                }
                else if (map[cave.Ypos - 31, cave.Xpos] != Terrain.MOUNTAIN && cave.Ypos - 28 < MAP_COLS && GetLocationByCoords(Tuple.Create(cave.Ypos - 28, cave.Xpos)) == null)
                {
                    map[cave.Ypos - 31, cave.Xpos] = Terrain.ROCK;
                    map[cave.Ypos - 30, cave.Xpos] = Terrain.ROAD;
                    map[cave.Ypos - 29, cave.Xpos] = Terrain.CAVE;
                    if (cave.Ypos - 28 < MAP_ROWS)
                    {
                        map[cave.Ypos - 28, cave.Xpos] = Terrain.MOUNTAIN;
                    }
                    cave.Ypos++;
                    rockNum--;
                }
                cavePicked = cave.MemAddress;
            }
            else if (!connections.Keys.Contains(cave) && cave != cave1 && cave != cave2 && cave.MemAddress != cavePicked)
            {
                if (map[cave.Ypos - 30, cave.Xpos - 1] != Terrain.MOUNTAIN)
                {
                    map[cave.Ypos - 30, cave.Xpos - 1] = Terrain.ROCK;
                    rockNum--;
                }
                else if (map[cave.Ypos - 30, cave.Xpos + 1] != Terrain.MOUNTAIN)
                {
                    map[cave.Ypos - 30, cave.Xpos + 1] = Terrain.ROCK;
                    rockNum--;
                }
                else if (map[cave.Ypos - 29, cave.Xpos] != Terrain.MOUNTAIN)
                {
                    map[cave.Ypos - 29, cave.Xpos] = Terrain.ROCK;
                    rockNum--;
                }
                else if (map[cave.Ypos - 31, cave.Xpos] != Terrain.MOUNTAIN)
                {
                    map[cave.Ypos - 31, cave.Xpos] = Terrain.ROCK;
                    rockNum--;
                }
                cavePicked = cave.MemAddress;

            }
        }
    }

    private void DrawMountains()
    {
        //create some mountains
        int mounty = hyrule.RNG.Next(22, 42);
        map[mounty, 0] = Terrain.MOUNTAIN;
        bool placedRoad = false;


        int endmounty = hyrule.RNG.Next(22, 42);
        int endmountx = hyrule.RNG.Next(2, 8);
        int x2 = 0;
        int y2 = mounty;
        int placedRocks = 0;
        while (x2 != (MAP_COLS - endmountx) || y2 != endmounty)
        {
            if (Math.Abs(x2 - (MAP_COLS - endmountx)) >= Math.Abs(y2 - endmounty))
            {
                if (x2 > MAP_COLS - endmountx && x2 > 0)
                {
                    x2--;
                }
                else if (x2 < MAP_COLS - 1)
                {
                    x2++;
                }
            }
            else
            {
                if (y2 > endmounty && y2 > 0)
                {
                    y2--;
                }
                else if (y2 < MAP_ROWS - 1)
                {
                    y2++;
                }
            }
            if (x2 != MAP_COLS - endmountx || y2 != endmounty)
            {
                if (map[y2, x2] == Terrain.NONE)
                {
                    map[y2, x2] = Terrain.MOUNTAIN;
                }
                else
                {
                    if (!placedRoad && map[y2, x2 + 1] != Terrain.ROAD)
                    {
                        if (hyrule.RNG.NextDouble() > .5 && (x2 > 0 && map[y2, x2 - 1] != Terrain.ROCK) && (x2 < MAP_COLS - 1 && map[y2, x2 + 1] != Terrain.ROCK) && (((y2 > 0 && map[y2 - 1, x2] == Terrain.ROAD) && (y2 < MAP_ROWS - 1 && map[y2 + 1, x2] == Terrain.ROAD)) || ((x2 > 0 && map[y2, x2 - 0] == Terrain.ROAD) && (x2 < MAP_COLS - 1 && map[y2, x2 + 1] == Terrain.ROAD))))
                        {
                            Location roadEnc = GetLocationByMem(0x4636);
                            roadEnc.Xpos = x2;
                            roadEnc.Ypos = y2 + 30;
                            roadEnc.CanShuffle = false;
                            roadEnc.Reachable = true;
                            placedRoad = true;
                        }
                        else if (placedRocks < 1)
                        {
                            Location roadEnc = GetLocationByMem(0x4636);
                            if ((roadEnc.Ypos - 30 != y2 && roadEnc.Xpos - 1 != x2) && (roadEnc.Ypos - 30 + 1 != y2 && roadEnc.Xpos != x2) && (roadEnc.Ypos - 30 - 1 != y2 && roadEnc.Xpos != x2) && (roadEnc.Ypos - 30 != y2 && roadEnc.Xpos + 1 != x2))
                            {
                                map[y2, x2] = Terrain.ROCK;
                                placedRocks++;
                            }
                        }
                    }
                    else if (placedRocks < 1)
                    {

                        map[y2, x2] = Terrain.ROCK;
                        placedRocks++;
                    }
                }
            }
        }

        if (!placedRoad)
        {
            Location roadEnc = GetLocationByMem(0x4636);
            roadEnc.Xpos = 0;
            roadEnc.Ypos = 0;
            roadEnc.CanShuffle = false;
        }
    }


    public void UpdateVisit()
    {
        visitation[start.Ypos - 30, start.Xpos] = true;
        UpdateReachable();

        foreach (Location location in AllLocations)
        {
            if (location.Ypos > 30)
            {
                if (visitation[location.Ypos - 30, location.Xpos])
                {
                    location.Reachable = true;
                    if (connections.Keys.Contains(location))
                    {
                        Location l2 = connections[location];
                        if ((location.NeedBagu && (bagu.Reachable || hyrule.SpellGet[Spell.FAIRY] 
                            || (hyrule.SpellGet[Spell.FIRE] && hyrule.SpellGet[Spell.JUMP] && hyrule.Props.DashSpell))))
                        {
                            l2.Reachable = true;
                            visitation[l2.Ypos - 30, l2.Xpos] = true;
                        }

                        if (location.NeedFairy && hyrule.SpellGet[Spell.FAIRY])
                        {
                            l2.Reachable = true;
                            visitation[l2.Ypos - 30, l2.Xpos] = true;
                        }

                        if (location.NeedJump && (hyrule.SpellGet[Spell.JUMP] || hyrule.SpellGet[Spell.FAIRY]))
                        {
                            l2.Reachable = true;
                            visitation[l2.Ypos - 30, l2.Xpos] = true;
                        }

                        if (!location.NeedFairy && !location.NeedBagu && !location.NeedJump)
                        {
                            l2.Reachable = true;
                            visitation[l2.Ypos - 30, l2.Xpos] = true;
                        }
                    }
                }
            }
        }
        if (lifeNorth.Reachable && lifeNorth.TownNum == Town.NEW_KASUTO)
        {
            lifeSouth.Reachable = true;
        }
    }

    protected override List<Location> GetPathingStarts()
    {
        return connections.Keys.Where(i => i.Reachable)
            .Union(new List<Location>() { start })
            .Union(GetContinentConnections().Where(i => i.Reachable))
            .ToList();
    }

    public override string GetName()
    {
        return "West";
    }
}
