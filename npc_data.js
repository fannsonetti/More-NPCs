// Auto-generated from NPCs/*.cs - run: python scripts/generate_npc_docs.py
// Do not edit manually.

const NPC_DATA = [
  {
    "file": "DominicCross.cs",
    "type": "supervisor",
    "description": "",
    "id": "dominic_cross",
    "firstName": "Dominic",
    "lastName": "Cross",
    "name": "Dominic Cross",
    "isDealer": false,
    "isManager": false,
    "isSupervisor": true,
    "region": "Downtown",
    "cut": 10.0
  },
  {
    "file": "SilasMercer.cs",
    "type": "supervisor",
    "description": "",
    "id": "silas_mercer",
    "firstName": "Silas",
    "lastName": "Mercer",
    "name": "Silas Mercer",
    "isDealer": false,
    "isManager": false,
    "isSupervisor": true,
    "region": "Uptown",
    "cut": 10.0
  },
  {
    "file": "ThomasAshford.cs",
    "type": "manager",
    "description": "",
    "id": "thomas_ashford",
    "firstName": "Thomas",
    "lastName": "Ashford",
    "name": "Thomas Ashford",
    "isDealer": false,
    "isManager": true,
    "isSupervisor": false,
    "region": "Suburbia",
    "cut": 10.0
  },
  {
    "file": "FannsoNetti.cs",
    "type": "dealer",
    "description": "",
    "id": "fannsonetti",
    "firstName": "FannsoNetti",
    "lastName": "",
    "name": "FannsoNetti",
    "isDealer": true,
    "isManager": false,
    "isSupervisor": false,
    "region": "Northtown",
    "cut": 20.0,
    "signingFee": 0,
    "home": "NorthWarehouse"
  },
  {
    "file": "MaxineJunefield.cs",
    "type": "dealer",
    "description": "",
    "id": "maxine_junefield",
    "firstName": "Maxine",
    "lastName": "Junefield",
    "name": "Maxine Junefield",
    "isDealer": true,
    "isManager": false,
    "isSupervisor": false,
    "region": "Westville",
    "cut": 20.0,
    "signingFee": 2500,
    "home": "Caravan"
  },
  {
    "file": "RickTorres.cs",
    "type": "dealer",
    "description": "",
    "id": "rick_torres",
    "firstName": "Rick",
    "lastName": "Torres",
    "name": "Rick Torres",
    "isDealer": true,
    "isManager": false,
    "isSupervisor": false,
    "region": "Downtown",
    "cut": 20.0,
    "signingFee": 1500,
    "home": "Supermarket"
  },
  {
    "file": "SergeantGrey.cs",
    "type": "dealer",
    "description": "",
    "id": "sergeant_grey",
    "firstName": "Sergeant",
    "lastName": "Grey",
    "name": "Sergeant Grey",
    "isDealer": true,
    "isManager": false,
    "isSupervisor": false,
    "region": "Suburbia",
    "cut": 20.0,
    "signingFee": 7500,
    "home": "PoliceStation"
  },
  {
    "file": "BenDover.cs",
    "type": "customer",
    "description": "",
    "id": "ben_dover",
    "firstName": "Ben",
    "lastName": "Dover",
    "name": "Ben Dover",
    "isDealer": false,
    "isManager": false,
    "isSupervisor": false,
    "region": "Northtown",
    "spending": [
      350,
      650
    ],
    "orders": [
      2,
      4
    ],
    "time": "19:40",
    "standards": "Low",
    "standardsDisplay": "Low",
    "day": "Wed",
    "affinities": {
      "Marijuana": 21,
      "Methamphetamine": 35,
      "Shrooms": -14,
      "Cocaine": 10
    },
    "properties": [
      "Calming",
      "Refreshing",
      "Smelly"
    ],
    "noProps": false,
    "policeRisk": 16,
    "dependence": 0.2,
    "connections": [
      "tess_tickle",
      "wayne_kerr"
    ],
    "unlocked": [
      "tess_tickle",
      "wayne_kerr"
    ],
    "avatar": "\ud83e\uddd1"
  },
  {
    "file": "BobbyCooley.cs",
    "type": "customer",
    "description": "",
    "id": "bobby_cooley",
    "firstName": "Bobby",
    "lastName": "Cooley",
    "name": "Bobby Cooley",
    "isDealer": false,
    "isManager": false,
    "isSupervisor": false,
    "region": "Westville",
    "spending": [
      50,
      250
    ],
    "orders": [
      2,
      4
    ],
    "time": "13:30",
    "standards": "VeryLow",
    "standardsDisplay": "Very Low",
    "day": "Wed",
    "affinities": {
      "Marijuana": 100,
      "Methamphetamine": 100,
      "Shrooms": 100,
      "Cocaine": 100
    },
    "properties": [],
    "noProps": true,
    "policeRisk": 10,
    "dependence": 0.0,
    "connections": [
      "meg_cooley"
    ],
    "unlocked": [
      "meg_cooley"
    ],
    "avatar": "\ud83e\uddd1"
  },
  {
    "file": "BrentHalver.cs",
    "type": "customer",
    "description": "",
    "id": "brent_halver",
    "firstName": "Brent",
    "lastName": "Halver",
    "name": "Brent Halver",
    "isDealer": false,
    "isManager": false,
    "isSupervisor": false,
    "region": "Westville",
    "spending": [
      400,
      600
    ],
    "orders": [
      2,
      4
    ],
    "time": "20:00",
    "standards": "Low",
    "standardsDisplay": "Low",
    "day": "Wed",
    "affinities": {
      "Marijuana": 52,
      "Methamphetamine": -8,
      "Shrooms": 10,
      "Cocaine": -25
    },
    "properties": [
      "Munchies",
      "Calming",
      "Energizing"
    ],
    "noProps": false,
    "policeRisk": 14,
    "dependence": 1,
    "connections": [
      "george_greene",
      "charles_rowland"
    ],
    "unlocked": [
      "george_greene",
      "charles_rowland"
    ],
    "avatar": "\ud83e\uddd1"
  },
  {
    "file": "BryceSherman.cs",
    "type": "customer",
    "description": "",
    "id": "bryce_sherman",
    "firstName": "Bryce",
    "lastName": "Sherman",
    "name": "Bryce Sherman",
    "isDealer": false,
    "isManager": false,
    "isSupervisor": false,
    "region": "Westville",
    "spending": [
      50,
      250
    ],
    "orders": [
      1,
      2
    ],
    "time": "16:30",
    "standards": "Moderate",
    "standardsDisplay": "Moderate",
    "day": "Fri",
    "affinities": {
      "Marijuana": 100,
      "Methamphetamine": 100,
      "Shrooms": 100,
      "Cocaine": 100
    },
    "properties": [],
    "noProps": true,
    "policeRisk": 18,
    "dependence": 1,
    "connections": [
      "victor_hughes",
      "bobby_cooley"
    ],
    "unlocked": [
      "victor_hughes",
      "bobby_cooley"
    ],
    "avatar": "\ud83e\uddd1"
  },
  {
    "file": "Corkskrew.cs",
    "type": "customer",
    "description": "",
    "id": "corkskrew",
    "firstName": "Corkskrew",
    "lastName": "",
    "name": "Corkskrew",
    "isDealer": false,
    "isManager": false,
    "isSupervisor": false,
    "region": "Westville",
    "spending": [
      400,
      600
    ],
    "orders": [
      1,
      4
    ],
    "time": "23:00",
    "standards": "Low",
    "standardsDisplay": "Low",
    "day": "Thu",
    "affinities": {
      "Marijuana": 52,
      "Methamphetamine": 73,
      "Shrooms": -43,
      "Cocaine": 14
    },
    "properties": [
      "AntiGravity",
      "Spicy",
      "CalorieDense"
    ],
    "noProps": false,
    "policeRisk": 19,
    "dependence": 0.25,
    "connections": [
      "trent_sherman",
      "keith_wagner"
    ],
    "unlocked": [
      "trent_sherman",
      "keith_wagner"
    ],
    "avatar": "\ud83e\uddd1"
  },
  {
    "file": "CoryLubbin.cs",
    "type": "customer",
    "description": "",
    "id": "cory_lubbin",
    "firstName": "Cory",
    "lastName": "Lubbin",
    "name": "Cory Lubbin",
    "isDealer": false,
    "isManager": false,
    "isSupervisor": false,
    "region": "Westville",
    "spending": [
      400,
      600
    ],
    "orders": [
      2,
      4
    ],
    "time": "15:00",
    "standards": "VeryLow",
    "standardsDisplay": "Very Low",
    "day": "Sat",
    "affinities": {
      "Marijuana": 38,
      "Methamphetamine": -14,
      "Shrooms": 18,
      "Cocaine": -25
    },
    "properties": [
      "Energizing",
      "Refreshing",
      "Sneaky"
    ],
    "noProps": false,
    "policeRisk": 9,
    "dependence": 1,
    "connections": [
      "bobby_cooley"
    ],
    "unlocked": [
      "bobby_cooley"
    ],
    "avatar": "\ud83e\uddd1"
  },
  {
    "file": "DanielJDalby.cs",
    "type": "customer",
    "description": "",
    "id": "daniel_j_dalby",
    "firstName": "Daniel J.",
    "lastName": "D'alby",
    "name": "Daniel J. D'alby",
    "isDealer": false,
    "isManager": false,
    "isSupervisor": false,
    "region": "Uptown",
    "spending": [
      1000,
      2000
    ],
    "orders": [
      1,
      3
    ],
    "time": "11:00",
    "standards": "High",
    "standardsDisplay": "High",
    "day": "Fri",
    "affinities": {
      "Marijuana": -52,
      "Methamphetamine": -86,
      "Shrooms": -62,
      "Cocaine": 14
    },
    "properties": [
      "Slippery",
      "Electrifying",
      "BrightEyed"
    ],
    "noProps": false,
    "policeRisk": 28,
    "dependence": 0.25,
    "connections": [
      "ray_hoffman",
      "lily_turner"
    ],
    "unlocked": [
      "ray_hoffman",
      "lily_turner"
    ],
    "avatar": "\ud83e\uddd1"
  },
  {
    "file": "DariusCole.cs",
    "type": "customer",
    "description": "",
    "id": "darius_cole",
    "firstName": "Darius",
    "lastName": "Cole",
    "name": "Darius Cole",
    "isDealer": false,
    "isManager": false,
    "isSupervisor": false,
    "region": "Uptown",
    "spending": [
      1000,
      2000
    ],
    "orders": [
      1,
      4
    ],
    "time": "19:30",
    "standards": "High",
    "standardsDisplay": "High",
    "day": "Tue",
    "affinities": {
      "Marijuana": -13,
      "Methamphetamine": 87,
      "Shrooms": 49,
      "Cocaine": -17
    },
    "properties": [
      "Laxative",
      "Shrinking",
      "Zombifying"
    ],
    "noProps": false,
    "policeRisk": 31,
    "dependence": 0.25,
    "connections": [
      "irene_meadows",
      "michael_boog"
    ],
    "unlocked": [
      "irene_meadows",
      "michael_boog"
    ],
    "avatar": "\ud83e\uddd1"
  },
  {
    "file": "DarlaCrane.cs",
    "type": "customer",
    "description": "",
    "id": "darla_crane",
    "firstName": "Darla",
    "lastName": "Crane",
    "name": "Darla Crane",
    "isDealer": false,
    "isManager": false,
    "isSupervisor": false,
    "region": "Westville",
    "spending": [
      250,
      550
    ],
    "orders": [
      1,
      3
    ],
    "time": "19:45",
    "standards": "Low",
    "standardsDisplay": "Low",
    "day": "Thu",
    "affinities": {
      "Marijuana": 28,
      "Methamphetamine": 91,
      "Shrooms": -42,
      "Cocaine": -12
    },
    "properties": [
      "Energizing",
      "Smelly",
      "Paranoia"
    ],
    "noProps": false,
    "policeRisk": 19,
    "dependence": 1,
    "connections": [
      "dean_webster",
      "shirley_watts"
    ],
    "unlocked": [
      "dean_webster",
      "shirley_watts"
    ],
    "avatar": "\ud83e\uddd1"
  },
  {
    "file": "DerekVale.cs",
    "type": "customer",
    "description": "",
    "id": "derek_vale",
    "firstName": "Derek",
    "lastName": "Vale",
    "name": "Derek Vale",
    "isDealer": false,
    "isManager": false,
    "isSupervisor": false,
    "region": "Northtown",
    "spending": [
      300,
      700
    ],
    "orders": [
      2,
      4
    ],
    "time": "20:15",
    "standards": "Low",
    "standardsDisplay": "Low",
    "day": "Mon",
    "affinities": {
      "Marijuana": 68,
      "Methamphetamine": -18,
      "Shrooms": 92,
      "Cocaine": -32
    },
    "properties": [
      "Calming",
      "Euphoric",
      "Glowing"
    ],
    "noProps": false,
    "policeRisk": 15,
    "dependence": 0.14,
    "connections": [
      "jason_reed",
      "austin_steiner"
    ],
    "unlocked": [
      "jason_reed",
      "austin_steiner"
    ],
    "avatar": "\ud83e\uddd1"
  },
  {
    "file": "Diesel.cs",
    "type": "customer",
    "description": "",
    "id": "diesel",
    "firstName": "Diesel",
    "lastName": "",
    "name": "Diesel",
    "isDealer": false,
    "isManager": false,
    "isSupervisor": false,
    "region": "Docks",
    "spending": [
      600,
      900
    ],
    "orders": [
      1,
      4
    ],
    "time": "00:30",
    "standards": "Moderate",
    "standardsDisplay": "Moderate",
    "day": "Mon",
    "affinities": {
      "Marijuana": -12,
      "Methamphetamine": 78,
      "Shrooms": -85,
      "Cocaine": 41
    },
    "properties": [
      "Energizing",
      "Paranoia",
      "Explosive"
    ],
    "noProps": false,
    "policeRisk": 22,
    "dependence": 0.0,
    "connections": [
      "melissa_wood",
      "jane_lucero"
    ],
    "unlocked": [
      "melissa_wood",
      "jane_lucero"
    ],
    "avatar": "\ud83e\uddd1"
  },
  {
    "file": "DorothySamwell.cs",
    "type": "customer",
    "description": "",
    "id": "dorothy_samwell",
    "firstName": "Dorothy",
    "lastName": "Samwell",
    "name": "Dorothy Samwell",
    "isDealer": false,
    "isManager": false,
    "isSupervisor": false,
    "region": "Northtown",
    "spending": [
      200,
      400
    ],
    "orders": [
      0,
      1
    ],
    "time": "14:00",
    "standards": "Low",
    "standardsDisplay": "Low",
    "day": "Sun",
    "affinities": {
      "Marijuana": 20,
      "Methamphetamine": -50,
      "Shrooms": 10,
      "Cocaine": -60
    },
    "properties": [
      "Calming",
      "Refreshing",
      "Focused"
    ],
    "noProps": false,
    "policeRisk": 14,
    "dependence": 0.0,
    "connections": [
      "beth_penn",
      "peggy_myers"
    ],
    "unlocked": [
      "beth_penn",
      "peggy_myers"
    ],
    "avatar": "\ud83e\uddd1"
  },
  {
    "file": "EarlHaskins.cs",
    "type": "customer",
    "description": "",
    "id": "earl_haskins",
    "firstName": "Earl",
    "lastName": "Haskins",
    "name": "Earl Haskins",
    "isDealer": false,
    "isManager": false,
    "isSupervisor": false,
    "region": "Westville",
    "spending": [
      500,
      700
    ],
    "orders": [
      1,
      3
    ],
    "time": "17:40",
    "standards": "Low",
    "standardsDisplay": "Low",
    "day": "Mon",
    "affinities": {
      "Marijuana": 24,
      "Methamphetamine": 8,
      "Shrooms": -18,
      "Cocaine": 30
    },
    "properties": [
      "Energizing",
      "Sedating",
      "Smelly"
    ],
    "noProps": false,
    "policeRisk": 15,
    "dependence": 1,
    "connections": [
      "dean_webster",
      "marlene_haskins"
    ],
    "unlocked": [
      "dean_webster",
      "marlene_haskins"
    ],
    "avatar": "\ud83e\uddd1"
  },
  {
    "file": "EdwardBoog.cs",
    "type": "customer",
    "description": "",
    "id": "edward_boog",
    "firstName": "Edward",
    "lastName": "Boog",
    "name": "Edward Boog",
    "isDealer": false,
    "isManager": false,
    "isSupervisor": false,
    "region": "Uptown",
    "spending": [
      2000,
      8000
    ],
    "orders": [
      1,
      3
    ],
    "time": "21:00",
    "standards": "VeryHigh",
    "standardsDisplay": "VeryHigh",
    "day": "Tue",
    "affinities": {
      "Marijuana": -77,
      "Methamphetamine": 21,
      "Shrooms": -87,
      "Cocaine": 74
    },
    "properties": [
      "Lethal",
      "Schizophrenic",
      "Explosive"
    ],
    "noProps": false,
    "policeRisk": 60,
    "dependence": 0.0,
    "connections": [
      "michael_boog",
      "tobias_wentworth"
    ],
    "unlocked": [
      "michael_boog",
      "tobias_wentworth"
    ],
    "avatar": "\ud83e\uddd1"
  },
  {
    "file": "EleanorBriggs.cs",
    "type": "customer",
    "description": "",
    "id": "eleanor_briggs",
    "firstName": "Eleanor",
    "lastName": "Briggs",
    "name": "Eleanor Briggs",
    "isDealer": false,
    "isManager": false,
    "isSupervisor": false,
    "region": "Downtown",
    "spending": [
      600,
      1000
    ],
    "orders": [
      1,
      4
    ],
    "time": "08:00",
    "standards": "Moderate",
    "standardsDisplay": "Moderate",
    "day": "Sat",
    "affinities": {
      "Marijuana": 53,
      "Methamphetamine": 92,
      "Shrooms": -46,
      "Cocaine": -24
    },
    "properties": [
      "Jennerising",
      "Gingeritis",
      "TropicThunder"
    ],
    "noProps": false,
    "policeRisk": 19,
    "dependence": 0.0,
    "connections": [
      "randy_caulfield"
    ],
    "unlocked": [
      "randy_caulfield"
    ],
    "avatar": "\ud83e\uddd1"
  },
  {
    "file": "EvanRowland.cs",
    "type": "customer",
    "description": "",
    "id": "evan_rowland",
    "firstName": "Evan",
    "lastName": "Rowland",
    "name": "Evan Rowland",
    "isDealer": false,
    "isManager": false,
    "isSupervisor": false,
    "region": "Westville",
    "spending": [
      50,
      250
    ],
    "orders": [
      1,
      2
    ],
    "time": "15:30",
    "standards": "VeryLow",
    "standardsDisplay": "Very Low",
    "day": "Fri",
    "affinities": {
      "Marijuana": 10,
      "Methamphetamine": -40,
      "Shrooms": 20,
      "Cocaine": -50
    },
    "properties": [
      "Calming",
      "Refreshing",
      "Focused"
    ],
    "noProps": false,
    "policeRisk": 9,
    "dependence": 1,
    "connections": [
      "bobby_cooley"
    ],
    "unlocked": [
      "bobby_cooley"
    ],
    "avatar": "\ud83e\uddd1"
  },
  {
    "file": "FinnMurphy.cs",
    "type": "customer",
    "description": "",
    "id": "finn_murphy",
    "firstName": "Finn",
    "lastName": "Murphy",
    "name": "Finn Murphy",
    "isDealer": false,
    "isManager": false,
    "isSupervisor": false,
    "region": "Docks",
    "spending": [
      600,
      900
    ],
    "orders": [
      1,
      4
    ],
    "time": "11:00",
    "standards": "Moderate",
    "standardsDisplay": "Moderate",
    "day": "Sat",
    "affinities": {
      "Marijuana": -35,
      "Methamphetamine": 57,
      "Shrooms": 12,
      "Cocaine": 21
    },
    "properties": [
      "Euphoric",
      "Paranoia",
      "Glowing"
    ],
    "noProps": false,
    "policeRisk": 21,
    "dependence": 0.2,
    "connections": [
      "kelly_reynolds",
      "lisa_gardener"
    ],
    "unlocked": [
      "kelly_reynolds",
      "lisa_gardener"
    ],
    "avatar": "\ud83e\uddd1"
  },
  {
    "file": "GavinHolt.cs",
    "type": "customer",
    "description": "",
    "id": "gavin_holt",
    "firstName": "Gavin",
    "lastName": "Holt",
    "name": "Gavin Holt",
    "isDealer": false,
    "isManager": false,
    "isSupervisor": false,
    "region": "Downtown",
    "spending": [
      400,
      600
    ],
    "orders": [
      1,
      3
    ],
    "time": "21:45",
    "standards": "Moderate",
    "standardsDisplay": "Moderate",
    "day": "Sun",
    "affinities": {
      "Marijuana": 10,
      "Methamphetamine": 63,
      "Shrooms": -20,
      "Cocaine": -26
    },
    "properties": [
      "Energizing",
      "Smelly",
      "Sneaky"
    ],
    "noProps": false,
    "policeRisk": 20,
    "dependence": 1,
    "connections": [
      "philip_wentworth",
      "greg_fliggle"
    ],
    "unlocked": [
      "philip_wentworth",
      "greg_fliggle"
    ],
    "avatar": "\ud83e\uddd1"
  },
  {
    "file": "Grunk.cs",
    "type": "customer",
    "description": "",
    "id": "grunk",
    "firstName": "Grunk",
    "lastName": "",
    "name": "Grunk",
    "isDealer": false,
    "isManager": false,
    "isSupervisor": false,
    "region": "Docks",
    "spending": [
      600,
      900
    ],
    "orders": [
      1,
      4
    ],
    "time": "20:00",
    "standards": "VeryLow",
    "standardsDisplay": "Very Low",
    "day": "Wed",
    "affinities": {
      "Marijuana": -68,
      "Methamphetamine": 86,
      "Shrooms": -2,
      "Cocaine": -85
    },
    "properties": [
      "Munchies",
      "Paranoia",
      "Toxic"
    ],
    "noProps": false,
    "policeRisk": 14,
    "dependence": 0.1,
    "connections": [
      "manhole_mike"
    ],
    "unlocked": [
      "manhole_mike"
    ],
    "avatar": "\ud83e\uddd1"
  },
  {
    "file": "HenryMitchell.cs",
    "type": "customer",
    "description": "",
    "id": "henry_mitchell",
    "firstName": "Henry",
    "lastName": "Mitchell",
    "name": "Henry Mitchell",
    "isDealer": false,
    "isManager": false,
    "isSupervisor": false,
    "region": "Docks",
    "spending": [
      700,
      1000
    ],
    "orders": [
      1,
      4
    ],
    "time": "12:00",
    "standards": "Low",
    "standardsDisplay": "Low",
    "day": "Thu",
    "affinities": {
      "Marijuana": 15,
      "Methamphetamine": -52,
      "Shrooms": 79,
      "Cocaine": 64
    },
    "properties": [
      "Calming",
      "ThoughtProvoking",
      "TropicThunder"
    ],
    "noProps": false,
    "policeRisk": 16,
    "dependence": 0.0,
    "connections": [
      "jane_lucero",
      "billy_kramer"
    ],
    "unlocked": [
      "jane_lucero",
      "billy_kramer"
    ],
    "avatar": "\ud83e\uddd1"
  },
  {
    "file": "JamalBennett.cs",
    "type": "customer",
    "description": "",
    "id": "jamal_bennett",
    "firstName": "Jamal",
    "lastName": "Bennett",
    "name": "Jamal Bennett",
    "isDealer": false,
    "isManager": false,
    "isSupervisor": false,
    "region": "Westville",
    "spending": [
      500,
      700
    ],
    "orders": [
      2,
      4
    ],
    "time": "08:00",
    "standards": "Moderate",
    "standardsDisplay": "Moderate",
    "day": "Sun",
    "affinities": {
      "Marijuana": 1,
      "Methamphetamine": 73,
      "Shrooms": 54,
      "Cocaine": -31
    },
    "properties": [
      "Cyclopean",
      "Balding",
      "Disorienting"
    ],
    "noProps": false,
    "policeRisk": 23,
    "dependence": 0.25,
    "connections": [
      "trent_sherman"
    ],
    "unlocked": [
      "trent_sherman"
    ],
    "avatar": "\ud83e\uddd1"
  },
  {
    "file": "JasonReed.cs",
    "type": "customer",
    "description": "",
    "id": "jason_reed",
    "firstName": "Jason",
    "lastName": "Reed",
    "name": "Jason Reed",
    "isDealer": false,
    "isManager": false,
    "isSupervisor": false,
    "region": "Northtown",
    "spending": [
      400,
      800
    ],
    "orders": [
      2,
      4
    ],
    "time": "20:20",
    "standards": "VeryLow",
    "standardsDisplay": "Very Low",
    "day": "Mon",
    "affinities": {
      "Marijuana": 86,
      "Methamphetamine": 25,
      "Shrooms": 100,
      "Cocaine": -67
    },
    "properties": [
      "LongFaced",
      "CalorieDense",
      "Shrinking"
    ],
    "noProps": false,
    "policeRisk": 11,
    "dependence": 0.1,
    "connections": [
      "kyle_cooley",
      "austin_steiner"
    ],
    "unlocked": [
      "kyle_cooley",
      "austin_steiner"
    ],
    "avatar": "\ud83e\uddd1"
  },
  {
    "file": "JianMing.cs",
    "type": "customer",
    "description": "",
    "id": "jian_ming",
    "firstName": "Jian",
    "lastName": "Ming",
    "name": "Jian Ming",
    "isDealer": false,
    "isManager": false,
    "isSupervisor": false,
    "region": "Northtown",
    "spending": [
      300,
      600
    ],
    "orders": [
      2,
      4
    ],
    "time": "16:00",
    "standards": "Moderate",
    "standardsDisplay": "Moderate",
    "day": "Thu",
    "affinities": {
      "Marijuana": 33,
      "Methamphetamine": -57,
      "Shrooms": 41,
      "Cocaine": -22
    },
    "properties": [
      "Calming",
      "Refreshing",
      "ThoughtProvoking"
    ],
    "noProps": false,
    "policeRisk": 21,
    "dependence": 0.25,
    "connections": [
      "ludwig_meyer",
      "fannsonetti"
    ],
    "unlocked": [
      "ludwig_meyer",
      "fannsonetti"
    ],
    "avatar": "\ud83e\uddd1"
  },
  {
    "file": "JosephWilkinson.cs",
    "type": "customer",
    "description": "",
    "id": "joseph_wilkinson",
    "firstName": "Joseph",
    "lastName": "Wilkinson",
    "name": "Joseph Wilkinson",
    "isDealer": false,
    "isManager": false,
    "isSupervisor": false,
    "region": "Downtown",
    "spending": [
      500,
      900
    ],
    "orders": [
      2,
      4
    ],
    "time": "11:00",
    "standards": "Moderate",
    "standardsDisplay": "Moderate",
    "day": "Sat",
    "affinities": {
      "Marijuana": -6,
      "Methamphetamine": 57,
      "Shrooms": 88,
      "Cocaine": 96
    },
    "properties": [
      "Zombifying",
      "Spicy",
      "Sedating"
    ],
    "noProps": false,
    "policeRisk": 21,
    "dependence": 0.25,
    "connections": [
      "elizabeth_homley",
      "fungal_phil"
    ],
    "unlocked": [
      "elizabeth_homley",
      "fungal_phil"
    ],
    "avatar": "\ud83e\uddd1"
  },
  {
    "file": "KaelaThorn.cs",
    "type": "customer",
    "description": "",
    "id": "kaela_thorn",
    "firstName": "Kaela",
    "lastName": "Thorn",
    "name": "Kaela Thorn",
    "isDealer": false,
    "isManager": false,
    "isSupervisor": false,
    "region": "Docks",
    "spending": [
      600,
      800
    ],
    "orders": [
      1,
      4
    ],
    "time": "09:30",
    "standards": "Moderate",
    "standardsDisplay": "Moderate",
    "day": "Sat",
    "affinities": {
      "Marijuana": -37,
      "Methamphetamine": 64,
      "Shrooms": -57,
      "Cocaine": 12
    },
    "properties": [
      "Smelly",
      "Seizure",
      "Foggy"
    ],
    "noProps": false,
    "policeRisk": 21,
    "dependence": 0.25,
    "connections": [
      "lisa_gardener",
      "anna_chesterfield"
    ],
    "unlocked": [
      "lisa_gardener",
      "anna_chesterfield"
    ],
    "avatar": "\ud83e\uddd1"
  },
  {
    "file": "KaraDempsey.cs",
    "type": "customer",
    "description": "",
    "id": "kara_dempsey",
    "firstName": "Kara",
    "lastName": "Dempsey",
    "name": "Kara Dempsey",
    "isDealer": false,
    "isManager": false,
    "isSupervisor": false,
    "region": "Westville",
    "spending": [
      400,
      600
    ],
    "orders": [
      1,
      3
    ],
    "time": "19:40",
    "standards": "Low",
    "standardsDisplay": "Low",
    "day": "Wed",
    "affinities": {
      "Marijuana": 44,
      "Methamphetamine": 6,
      "Shrooms": 18,
      "Cocaine": -12
    },
    "properties": [
      "Calming",
      "Refreshing",
      "Foggy"
    ],
    "noProps": false,
    "policeRisk": 14,
    "dependence": 1,
    "connections": [
      "rory_dempsey",
      "sarah_greene"
    ],
    "unlocked": [
      "rory_dempsey",
      "sarah_greene"
    ],
    "avatar": "\ud83e\uddd1"
  },
  {
    "file": "LenaHart.cs",
    "type": "customer",
    "description": "",
    "id": "lena_hart",
    "firstName": "Lena",
    "lastName": "Hart",
    "name": "Lena Hart",
    "isDealer": false,
    "isManager": false,
    "isSupervisor": false,
    "region": "Northtown",
    "spending": [
      400,
      900
    ],
    "orders": [
      1,
      3
    ],
    "time": "17:30",
    "standards": "Low",
    "standardsDisplay": "Low",
    "day": "Wed",
    "affinities": {
      "Marijuana": 18,
      "Methamphetamine": -21,
      "Shrooms": 39,
      "Cocaine": 28
    },
    "properties": [
      "Refreshing",
      "Calming",
      "ThoughtProvoking"
    ],
    "noProps": false,
    "policeRisk": 15,
    "dependence": 0.08,
    "connections": [
      "kathy_henderson",
      "donna_martin"
    ],
    "unlocked": [
      "kathy_henderson",
      "donna_martin"
    ],
    "avatar": "\ud83e\uddd1"
  },
  {
    "file": "LilaPark.cs",
    "type": "customer",
    "description": "",
    "id": "lila_park",
    "firstName": "Lila",
    "lastName": "Park",
    "name": "Lila Park",
    "isDealer": false,
    "isManager": false,
    "isSupervisor": false,
    "region": "Westville",
    "spending": [
      450,
      700
    ],
    "orders": [
      1,
      4
    ],
    "time": "18:20",
    "standards": "Moderate",
    "standardsDisplay": "Moderate",
    "day": "Sat",
    "affinities": {
      "Marijuana": 30,
      "Methamphetamine": -22,
      "Shrooms": 48,
      "Cocaine": 7
    },
    "properties": [
      "Sneaky",
      "Spicy",
      "BrightEyed"
    ],
    "noProps": false,
    "policeRisk": 19,
    "dependence": 1,
    "connections": [
      "nora_kessler",
      "maxine_junefield"
    ],
    "unlocked": [
      "nora_kessler",
      "maxine_junefield"
    ],
    "avatar": "\ud83e\uddd1"
  },
  {
    "file": "Mack.cs",
    "type": "customer",
    "description": "",
    "id": "mack",
    "firstName": "Mack",
    "lastName": "",
    "name": "Mack",
    "isDealer": false,
    "isManager": false,
    "isSupervisor": false,
    "region": "Docks",
    "spending": [
      600,
      900
    ],
    "orders": [
      1,
      4
    ],
    "time": "00:30",
    "standards": "Low",
    "standardsDisplay": "Low",
    "day": "Mon",
    "affinities": {
      "Marijuana": 32,
      "Methamphetamine": -45,
      "Shrooms": -73,
      "Cocaine": 67
    },
    "properties": [
      "Focused",
      "Sneaky",
      "Euphoric"
    ],
    "noProps": false,
    "policeRisk": 19,
    "dependence": 0.0,
    "connections": [
      "melissa_wood",
      "diesel"
    ],
    "unlocked": [
      "melissa_wood",
      "diesel"
    ],
    "avatar": "\ud83e\uddd1"
  },
  {
    "file": "ManholeMike.cs",
    "type": "customer",
    "description": "",
    "id": "manhole_mike",
    "firstName": "Manhole Mike",
    "lastName": "",
    "name": "Manhole Mike",
    "isDealer": false,
    "isManager": false,
    "isSupervisor": false,
    "region": "Docks",
    "spending": [
      600,
      800
    ],
    "orders": [
      1,
      3
    ],
    "time": "22:00",
    "standards": "VeryLow",
    "standardsDisplay": "Very Low",
    "day": "Mon",
    "affinities": {
      "Marijuana": -73,
      "Methamphetamine": 92,
      "Shrooms": 5,
      "Cocaine": -26
    },
    "properties": [
      "Paranoia",
      "Laxative",
      "Munchies"
    ],
    "noProps": false,
    "policeRisk": 12,
    "dependence": 0.0,
    "connections": [
      "cranky_frank",
      "mack"
    ],
    "unlocked": [
      "cranky_frank",
      "mack"
    ],
    "avatar": "\ud83e\uddd1"
  },
  {
    "file": "MarlaHale.cs",
    "type": "customer",
    "description": "",
    "id": "marla_hale",
    "firstName": "Marla",
    "lastName": "Hale",
    "name": "Marla Hale",
    "isDealer": false,
    "isManager": false,
    "isSupervisor": false,
    "region": "Northtown",
    "spending": [
      300,
      600
    ],
    "orders": [
      2,
      4
    ],
    "time": "19:45",
    "standards": "VeryLow",
    "standardsDisplay": "Very Low",
    "day": "Thu",
    "affinities": {
      "Marijuana": 18,
      "Methamphetamine": 71,
      "Shrooms": -22,
      "Cocaine": 24
    },
    "properties": [
      "Energizing",
      "Euphoric",
      "Foggy"
    ],
    "noProps": false,
    "policeRisk": 11,
    "dependence": 0.18,
    "connections": [
      "kyle_cooley",
      "nico_marlowe"
    ],
    "unlocked": [
      "kyle_cooley",
      "nico_marlowe"
    ],
    "avatar": "\ud83e\uddd1"
  },
  {
    "file": "MarleneHaskins.cs",
    "type": "customer",
    "description": "",
    "id": "marlene_haskins",
    "firstName": "Marlene",
    "lastName": "Haskins",
    "name": "Marlene Haskins",
    "isDealer": false,
    "isManager": false,
    "isSupervisor": false,
    "region": "Westville",
    "spending": [
      450,
      650
    ],
    "orders": [
      1,
      3
    ],
    "time": "18:10",
    "standards": "Low",
    "standardsDisplay": "Low",
    "day": "Fri",
    "affinities": {
      "Marijuana": 18,
      "Methamphetamine": -26,
      "Shrooms": 34,
      "Cocaine": 9
    },
    "properties": [
      "Glowing",
      "Focused",
      "ThoughtProvoking"
    ],
    "noProps": false,
    "policeRisk": 15,
    "dependence": 1,
    "connections": [
      "earl_haskins"
    ],
    "unlocked": [
      "earl_haskins"
    ],
    "avatar": "\ud83e\uddd1"
  },
  {
    "file": "MaxPennyson.cs",
    "type": "customer",
    "description": "",
    "id": "max_pennyson",
    "firstName": "Max",
    "lastName": "Pennyson",
    "name": "Max Pennyson",
    "isDealer": false,
    "isManager": false,
    "isSupervisor": false,
    "region": "Downtown",
    "spending": [
      600,
      900
    ],
    "orders": [
      1,
      4
    ],
    "time": "19:00",
    "standards": "Moderate",
    "standardsDisplay": "Moderate",
    "day": "Sat",
    "affinities": {
      "Marijuana": 45,
      "Methamphetamine": -52,
      "Shrooms": 62,
      "Cocaine": -28
    },
    "properties": [
      "Munchies",
      "Calming",
      "ThoughtProvoking"
    ],
    "noProps": false,
    "policeRisk": 21,
    "dependence": 0.2,
    "connections": [
      "bruce_norton",
      "philip_wentworth"
    ],
    "unlocked": [
      "bruce_norton",
      "philip_wentworth"
    ],
    "avatar": "\ud83e\uddd1"
  },
  {
    "file": "MayaWebb.cs",
    "type": "customer",
    "description": "",
    "id": "maya_webb",
    "firstName": "Maya",
    "lastName": "Webb",
    "name": "Maya Webb",
    "isDealer": false,
    "isManager": false,
    "isSupervisor": false,
    "region": "Docks",
    "spending": [
      600,
      1000
    ],
    "orders": [
      2,
      4
    ],
    "time": "18:30",
    "standards": "Moderate",
    "standardsDisplay": "Moderate",
    "day": "Wed",
    "affinities": {
      "Marijuana": 28,
      "Methamphetamine": -42,
      "Shrooms": 51,
      "Cocaine": -19
    },
    "properties": [
      "Calming",
      "Refreshing",
      "ThoughtProvoking"
    ],
    "noProps": false,
    "policeRisk": 21,
    "dependence": 0.2,
    "connections": [
      "mac_cooper",
      "billy_kramer"
    ],
    "unlocked": [
      "mac_cooper",
      "billy_kramer"
    ],
    "avatar": "\ud83e\uddd1"
  },
  {
    "file": "MiltonDelaney.cs",
    "type": "customer",
    "description": "",
    "id": "milton_delaney",
    "firstName": "Milton",
    "lastName": "Delaney",
    "name": "Milton Delaney",
    "isDealer": false,
    "isManager": false,
    "isSupervisor": false,
    "region": "Downtown",
    "spending": [
      600,
      1000
    ],
    "orders": [
      1,
      4
    ],
    "time": "11:00",
    "standards": "Moderate",
    "standardsDisplay": "Moderate",
    "day": "Sat",
    "affinities": {
      "Marijuana": 64,
      "Methamphetamine": 86,
      "Shrooms": -59,
      "Cocaine": -22
    },
    "properties": [
      "Munchies",
      "Refreshing",
      "Zombifying"
    ],
    "noProps": false,
    "policeRisk": 20,
    "dependence": 0.0,
    "connections": [
      "jeff_gilmore",
      "eleanor_briggs"
    ],
    "unlocked": [
      "jeff_gilmore",
      "eleanor_briggs"
    ],
    "avatar": "\ud83e\uddd1"
  },
  {
    "file": "MoeLester.cs",
    "type": "customer",
    "description": "",
    "id": "moe_lester",
    "firstName": "Moe",
    "lastName": "Lester",
    "name": "Moe Lester",
    "isDealer": false,
    "isManager": false,
    "isSupervisor": false,
    "region": "Northtown",
    "spending": [
      300,
      600
    ],
    "orders": [
      2,
      4
    ],
    "time": "14:00",
    "standards": "Low",
    "standardsDisplay": "Low",
    "day": "Wed",
    "affinities": {
      "Marijuana": 52,
      "Methamphetamine": -23,
      "Shrooms": 51,
      "Cocaine": 31
    },
    "properties": [
      "Jennerising",
      "Refreshing",
      "Glowing"
    ],
    "noProps": false,
    "policeRisk": 16,
    "dependence": 0.25,
    "connections": [
      "jessi_waters",
      "peter_file"
    ],
    "unlocked": [
      "jessi_waters",
      "peter_file"
    ],
    "avatar": "\ud83e\uddd1"
  },
  {
    "file": "MrMing.cs",
    "type": "customer",
    "description": "",
    "id": "mr_ming",
    "firstName": "Mr.",
    "lastName": "Ming",
    "name": "Mr. Ming",
    "isDealer": false,
    "isManager": false,
    "isSupervisor": false,
    "region": "Northtown",
    "spending": [
      300,
      700
    ],
    "orders": [
      2,
      4
    ],
    "time": "12:00",
    "standards": "Moderate",
    "standardsDisplay": "Moderate",
    "day": "Fri",
    "affinities": {
      "Marijuana": 30,
      "Methamphetamine": 20,
      "Shrooms": 26,
      "Cocaine": 10
    },
    "properties": [
      "Calming",
      "Refreshing",
      "ThoughtProvoking"
    ],
    "noProps": false,
    "policeRisk": 20,
    "dependence": 0.1,
    "connections": [
      "ming"
    ],
    "unlocked": [
      "ming"
    ],
    "avatar": "\ud83e\uddd1"
  },
  {
    "file": "NadiaParker.cs",
    "type": "customer",
    "description": "",
    "id": "nadia_parker",
    "firstName": "Nadia",
    "lastName": "Parker",
    "name": "Nadia Parker",
    "isDealer": false,
    "isManager": false,
    "isSupervisor": false,
    "region": "Downtown",
    "spending": [
      500,
      800
    ],
    "orders": [
      1,
      3
    ],
    "time": "19:30",
    "standards": "Moderate",
    "standardsDisplay": "Moderate",
    "day": "Tue",
    "affinities": {
      "Marijuana": 28,
      "Methamphetamine": 4,
      "Shrooms": 20,
      "Cocaine": -8
    },
    "properties": [
      "Refreshing",
      "Focused",
      "Sneaky"
    ],
    "noProps": false,
    "policeRisk": 22,
    "dependence": 1,
    "connections": [
      "eugene_buckley",
      "gavin_holt"
    ],
    "unlocked": [
      "eugene_buckley",
      "gavin_holt"
    ],
    "avatar": "\ud83e\uddd1"
  },
  {
    "file": "NicoMarlowe.cs",
    "type": "customer",
    "description": "",
    "id": "nico_marlowe",
    "firstName": "Nico",
    "lastName": "Marlowe",
    "name": "Nico Marlowe",
    "isDealer": false,
    "isManager": false,
    "isSupervisor": false,
    "region": "Northtown",
    "spending": [
      200,
      500
    ],
    "orders": [
      1,
      3
    ],
    "time": "19:30",
    "standards": "Low",
    "standardsDisplay": "Low",
    "day": "Thu",
    "affinities": {
      "Marijuana": 41,
      "Methamphetamine": 35,
      "Shrooms": -12,
      "Cocaine": -8
    },
    "properties": [
      "Euphoric",
      "Foggy",
      "Smelly"
    ],
    "noProps": false,
    "policeRisk": 15,
    "dependence": 0.17,
    "connections": [
      "owen_crowe"
    ],
    "unlocked": [
      "owen_crowe"
    ],
    "avatar": "\ud83e\uddd1"
  },
  {
    "file": "NoraKessler.cs",
    "type": "customer",
    "description": "",
    "id": "nora_kessler",
    "firstName": "Nora",
    "lastName": "Kessler",
    "name": "Nora Kessler",
    "isDealer": false,
    "isManager": false,
    "isSupervisor": false,
    "region": "Westville",
    "spending": [
      500,
      700
    ],
    "orders": [
      2,
      4
    ],
    "time": "16:15",
    "standards": "Moderate",
    "standardsDisplay": "Moderate",
    "day": "Thu",
    "affinities": {
      "Marijuana": -63,
      "Methamphetamine": 13,
      "Shrooms": 68,
      "Cocaine": 46
    },
    "properties": [
      "Smelly",
      "TropicThunder",
      "Gingeritis"
    ],
    "noProps": false,
    "policeRisk": 23,
    "dependence": 0.25,
    "connections": [
      "trent_sherman",
      "lila_park"
    ],
    "unlocked": [
      "trent_sherman",
      "lila_park"
    ],
    "avatar": "\ud83e\uddd1"
  },
  {
    "file": "OfficerMarcus.cs",
    "type": "customer",
    "description": "",
    "id": "officer_marcus",
    "firstName": "Officer",
    "lastName": "Marcus",
    "name": "Officer Marcus",
    "isDealer": false,
    "isManager": false,
    "isSupervisor": false,
    "region": "Suburbia",
    "spending": [
      800,
      1200
    ],
    "orders": [
      1,
      4
    ],
    "time": "23:30",
    "standards": "High",
    "standardsDisplay": "High",
    "day": "Sun",
    "affinities": {
      "Marijuana": 52,
      "Methamphetamine": -84,
      "Shrooms": 28,
      "Cocaine": -9
    },
    "properties": [
      "Athletic",
      "AntiGravity",
      "Sneaky"
    ],
    "noProps": false,
    "policeRisk": 75,
    "dependence": 0.0,
    "connections": [
      "alison_knight",
      "jack_knight"
    ],
    "unlocked": [
      "alison_knight",
      "jack_knight"
    ],
    "avatar": "\ud83e\uddd1"
  },
  {
    "file": "OwenCrowe.cs",
    "type": "customer",
    "description": "",
    "id": "owen_crowe",
    "firstName": "Owen",
    "lastName": "Crowe",
    "name": "Owen Crowe",
    "isDealer": false,
    "isManager": false,
    "isSupervisor": false,
    "region": "Northtown",
    "spending": [
      200,
      500
    ],
    "orders": [
      1,
      3
    ],
    "time": "22:15",
    "standards": "Low",
    "standardsDisplay": "Low",
    "day": "Sun",
    "affinities": {
      "Marijuana": 22,
      "Methamphetamine": 87,
      "Shrooms": -44,
      "Cocaine": -18
    },
    "properties": [
      "Energizing",
      "Smelly",
      "ThoughtProvoking"
    ],
    "noProps": false,
    "policeRisk": 16,
    "dependence": 0.26,
    "connections": [
      "kyle_cooley",
      "jason_reed"
    ],
    "unlocked": [
      "kyle_cooley",
      "jason_reed"
    ],
    "avatar": "\ud83e\uddd1"
  },
  {
    "file": "PiperSloan.cs",
    "type": "customer",
    "description": "",
    "id": "piper_sloan",
    "firstName": "Piper",
    "lastName": "Sloan",
    "name": "Piper Sloan",
    "isDealer": false,
    "isManager": false,
    "isSupervisor": false,
    "region": "Northtown",
    "spending": [
      300,
      700
    ],
    "orders": [
      1,
      4
    ],
    "time": "18:30",
    "standards": "Low",
    "standardsDisplay": "Low",
    "day": "Tue",
    "affinities": {
      "Marijuana": 57,
      "Methamphetamine": 12,
      "Shrooms": 72,
      "Cocaine": -28
    },
    "properties": [
      "Calming",
      "Euphoric",
      "Shrinking"
    ],
    "noProps": false,
    "policeRisk": 15,
    "dependence": 0.12,
    "connections": [
      "kyle_cooley"
    ],
    "unlocked": [
      "kyle_cooley"
    ],
    "avatar": "\ud83e\uddd1"
  },
  {
    "file": "PriyaMadden.cs",
    "type": "customer",
    "description": "",
    "id": "priya_madden",
    "firstName": "Priya",
    "lastName": "Madden",
    "name": "Priya Madden",
    "isDealer": false,
    "isManager": false,
    "isSupervisor": false,
    "region": "Downtown",
    "spending": [
      500,
      700
    ],
    "orders": [
      1,
      4
    ],
    "time": "18:30",
    "standards": "Moderate",
    "standardsDisplay": "Moderate",
    "day": "Tue",
    "affinities": {
      "Marijuana": 36,
      "Methamphetamine": 6,
      "Shrooms": 24,
      "Cocaine": -18
    },
    "properties": [
      "Refreshing",
      "Calming",
      "Glowing"
    ],
    "noProps": false,
    "policeRisk": 20,
    "dependence": 1,
    "connections": [
      "jennifer_rivera",
      "eugene_buckley"
    ],
    "unlocked": [
      "jennifer_rivera",
      "eugene_buckley"
    ],
    "avatar": "\ud83e\uddd1"
  },
  {
    "file": "RheaLarkin.cs",
    "type": "customer",
    "description": "",
    "id": "rhea_larkin",
    "firstName": "Rhea",
    "lastName": "Larkin",
    "name": "Rhea Larkin",
    "isDealer": false,
    "isManager": false,
    "isSupervisor": false,
    "region": "Downtown",
    "spending": [
      500,
      800
    ],
    "orders": [
      1,
      3
    ],
    "time": "20:00",
    "standards": "Moderate",
    "standardsDisplay": "Moderate",
    "day": "Thu",
    "affinities": {
      "Marijuana": 24,
      "Methamphetamine": 8,
      "Shrooms": 22,
      "Cocaine": -10
    },
    "properties": [
      "Refreshing",
      "Focused",
      "Glowing"
    ],
    "noProps": false,
    "policeRisk": 22,
    "dependence": 1,
    "connections": [
      "jennifer_rivera",
      "sienna_crowley"
    ],
    "unlocked": [
      "jennifer_rivera",
      "sienna_crowley"
    ],
    "avatar": "\ud83e\uddd1"
  },
  {
    "file": "RhondaVex.cs",
    "type": "customer",
    "description": "",
    "id": "rhonda_vex",
    "firstName": "Rhonda",
    "lastName": "Vex",
    "name": "Rhonda Vex",
    "isDealer": false,
    "isManager": false,
    "isSupervisor": false,
    "region": "Westville",
    "spending": [
      500,
      700
    ],
    "orders": [
      2,
      4
    ],
    "time": "20:00",
    "standards": "Moderate",
    "standardsDisplay": "Moderate",
    "day": "Thu",
    "affinities": {
      "Marijuana": 12,
      "Methamphetamine": 92,
      "Shrooms": -35,
      "Cocaine": 28
    },
    "properties": [
      "Energizing",
      "Smelly",
      "Paranoia"
    ],
    "noProps": false,
    "policeRisk": 20,
    "dependence": 0.22,
    "connections": [
      "charles_rowland",
      "kim_delaney"
    ],
    "unlocked": [
      "charles_rowland",
      "kim_delaney"
    ],
    "avatar": "\ud83e\uddd1"
  },
  {
    "file": "RoryDempsey.cs",
    "type": "customer",
    "description": "",
    "id": "rory_dempsey",
    "firstName": "Rory",
    "lastName": "Dempsey",
    "name": "Rory Dempsey",
    "isDealer": false,
    "isManager": false,
    "isSupervisor": false,
    "region": "Westville",
    "spending": [
      400,
      600
    ],
    "orders": [
      1,
      3
    ],
    "time": "19:30",
    "standards": "Low",
    "standardsDisplay": "Low",
    "day": "Mon",
    "affinities": {
      "Marijuana": 42,
      "Methamphetamine": 18,
      "Shrooms": -10,
      "Cocaine": -22
    },
    "properties": [
      "Calming",
      "Foggy",
      "ThoughtProvoking"
    ],
    "noProps": false,
    "policeRisk": 15,
    "dependence": 1,
    "connections": [
      "george_greene"
    ],
    "unlocked": [
      "george_greene"
    ],
    "avatar": "\ud83e\uddd1"
  },
  {
    "file": "SalRusso.cs",
    "type": "customer",
    "description": "",
    "id": "sal_russo",
    "firstName": "Sal",
    "lastName": "Russo",
    "name": "Sal Russo",
    "isDealer": false,
    "isManager": false,
    "isSupervisor": false,
    "region": "Docks",
    "spending": [
      700,
      1000
    ],
    "orders": [
      1,
      4
    ],
    "time": "19:30",
    "standards": "Low",
    "standardsDisplay": "Low",
    "day": "Tue",
    "affinities": {
      "Marijuana": 22,
      "Methamphetamine": 31,
      "Shrooms": -41,
      "Cocaine": -18
    },
    "properties": [
      "Calming",
      "Munchies",
      "Paranoia"
    ],
    "noProps": false,
    "policeRisk": 17,
    "dependence": 0.15,
    "connections": [
      "marco_baron",
      "sherman_giles"
    ],
    "unlocked": [
      "marco_baron",
      "sherman_giles"
    ],
    "avatar": "\ud83e\uddd1"
  },
  {
    "file": "SarahGreene.cs",
    "type": "customer",
    "description": "",
    "id": "sarah_greene",
    "firstName": "Sarah",
    "lastName": "Greene",
    "name": "Sarah Greene",
    "isDealer": false,
    "isManager": false,
    "isSupervisor": false,
    "region": "Westville",
    "spending": [
      800,
      2000
    ],
    "orders": [
      1,
      1
    ],
    "time": "12:45",
    "standards": "Moderate",
    "standardsDisplay": "Moderate",
    "day": "Sun",
    "affinities": {
      "Marijuana": 45,
      "Methamphetamine": 12,
      "Shrooms": 22,
      "Cocaine": 18
    },
    "properties": [
      "Refreshing",
      "Focused",
      "Glowing"
    ],
    "noProps": false,
    "policeRisk": 19,
    "dependence": 1,
    "connections": [
      "george_greene"
    ],
    "unlocked": [
      "george_greene"
    ],
    "avatar": "\ud83e\uddd1"
  },
  {
    "file": "SharonWebster.cs",
    "type": "customer",
    "description": "",
    "id": "sharon_webster",
    "firstName": "Sharon",
    "lastName": "Webster",
    "name": "Sharon Webster",
    "isDealer": false,
    "isManager": false,
    "isSupervisor": false,
    "region": "Westville",
    "spending": [
      350,
      650
    ],
    "orders": [
      1,
      3
    ],
    "time": "18:30",
    "standards": "Low",
    "standardsDisplay": "Low",
    "day": "Tue",
    "affinities": {
      "Marijuana": 22,
      "Methamphetamine": -16,
      "Shrooms": 10,
      "Cocaine": 31
    },
    "properties": [
      "CalorieDense",
      "Paranoia",
      "AntiGravity"
    ],
    "noProps": false,
    "policeRisk": 15,
    "dependence": 1,
    "connections": [
      "dean_webster",
      "marlene_haskins"
    ],
    "unlocked": [
      "dean_webster",
      "marlene_haskins"
    ],
    "avatar": "\ud83e\uddd1"
  },
  {
    "file": "SiennaCrowley.cs",
    "type": "customer",
    "description": "",
    "id": "sienna_crowley",
    "firstName": "Sienna",
    "lastName": "Crowley",
    "name": "Sienna Crowley",
    "isDealer": false,
    "isManager": false,
    "isSupervisor": false,
    "region": "Downtown",
    "spending": [
      500,
      700
    ],
    "orders": [
      1,
      4
    ],
    "time": "19:00",
    "standards": "Moderate",
    "standardsDisplay": "Moderate",
    "day": "Fri",
    "affinities": {
      "Marijuana": 18,
      "Methamphetamine": 24,
      "Shrooms": 34,
      "Cocaine": -21
    },
    "properties": [
      "Euphoric",
      "Refreshing",
      "Sedating"
    ],
    "noProps": false,
    "policeRisk": 20,
    "dependence": 1,
    "connections": [
      "lucy_pennington",
      "jennifer_rivera"
    ],
    "unlocked": [
      "lucy_pennington",
      "jennifer_rivera"
    ],
    "avatar": "\ud83e\uddd1"
  },
  {
    "file": "SkylerWilkinson.cs",
    "type": "customer",
    "description": "",
    "id": "skyler_wilkinson",
    "firstName": "Skyler",
    "lastName": "Wilkinson",
    "name": "Skyler Wilkinson",
    "isDealer": false,
    "isManager": false,
    "isSupervisor": false,
    "region": "Suburbia",
    "spending": [
      700,
      1200
    ],
    "orders": [
      1,
      3
    ],
    "time": "07:00",
    "standards": "High",
    "standardsDisplay": "High",
    "day": "Fri",
    "affinities": {
      "Marijuana": 39,
      "Methamphetamine": 21,
      "Shrooms": -54,
      "Cocaine": 77
    },
    "properties": [
      "Schizophrenic",
      "Glowing",
      "Toxic"
    ],
    "noProps": false,
    "policeRisk": 28,
    "dependence": 0.0,
    "connections": [
      "alison_knight",
      "officer_marcus"
    ],
    "unlocked": [
      "alison_knight",
      "officer_marcus"
    ],
    "avatar": "\ud83e\uddd1"
  },
  {
    "file": "TessTickle.cs",
    "type": "customer",
    "description": "",
    "id": "tess_tickle",
    "firstName": "Tess",
    "lastName": "Tickle",
    "name": "Tess Tickle",
    "isDealer": false,
    "isManager": false,
    "isSupervisor": false,
    "region": "Northtown",
    "spending": [
      400,
      700
    ],
    "orders": [
      2,
      4
    ],
    "time": "21:00",
    "standards": "Low",
    "standardsDisplay": "Low",
    "day": "Fri",
    "affinities": {
      "Marijuana": 12,
      "Methamphetamine": 9,
      "Shrooms": 56,
      "Cocaine": 34
    },
    "properties": [
      "Euphoric",
      "Glowing",
      "Refreshing"
    ],
    "noProps": false,
    "policeRisk": 15,
    "dependence": 0.16,
    "connections": [
      "peter_file",
      "wayne_kerr"
    ],
    "unlocked": [
      "peter_file",
      "wayne_kerr"
    ],
    "avatar": "\ud83e\uddd1"
  },
  {
    "file": "TrishaMorrow.cs",
    "type": "customer",
    "description": "",
    "id": "trisha_morrow",
    "firstName": "Trisha",
    "lastName": "Morrow",
    "name": "Trisha Morrow",
    "isDealer": false,
    "isManager": false,
    "isSupervisor": false,
    "region": "Westville",
    "spending": [
      200,
      500
    ],
    "orders": [
      1,
      3
    ],
    "time": "20:30",
    "standards": "Low",
    "standardsDisplay": "Low",
    "day": "Wed",
    "affinities": {
      "Marijuana": 22,
      "Methamphetamine": 94,
      "Shrooms": -35,
      "Cocaine": -18
    },
    "properties": [
      "Energizing",
      "Smelly",
      "Paranoia"
    ],
    "noProps": false,
    "policeRisk": 16,
    "dependence": 1,
    "connections": [
      "shirley_watts"
    ],
    "unlocked": [
      "shirley_watts"
    ],
    "avatar": "\ud83e\uddd1"
  },
  {
    "file": "ValerieVoss.cs",
    "type": "customer",
    "description": "",
    "id": "valerie_voss",
    "firstName": "Valerie",
    "lastName": "Voss",
    "name": "Valerie Voss",
    "isDealer": false,
    "isManager": false,
    "isSupervisor": false,
    "region": "Northtown",
    "spending": [
      400,
      700
    ],
    "orders": [
      2,
      4
    ],
    "time": "14:00",
    "standards": "Moderate",
    "standardsDisplay": "Moderate",
    "day": "Fri",
    "affinities": {
      "Marijuana": 47,
      "Methamphetamine": -23,
      "Shrooms": 61,
      "Cocaine": -51
    },
    "properties": [
      "Refreshing",
      "Calming",
      "Glowing"
    ],
    "noProps": false,
    "policeRisk": 21,
    "dependence": 0.25,
    "connections": [
      "vincent_reeves"
    ],
    "unlocked": [
      "vincent_reeves"
    ],
    "avatar": "\ud83e\uddd1"
  },
  {
    "file": "VictorHughes.cs",
    "type": "customer",
    "description": "",
    "id": "victor_hughes",
    "firstName": "Victor",
    "lastName": "Hughes",
    "name": "Victor Hughes",
    "isDealer": false,
    "isManager": false,
    "isSupervisor": false,
    "region": "Westville",
    "spending": [
      500,
      700
    ],
    "orders": [
      1,
      4
    ],
    "time": "19:00",
    "standards": "Moderate",
    "standardsDisplay": "Moderate",
    "day": "Sat",
    "affinities": {
      "Marijuana": 93,
      "Methamphetamine": -3,
      "Shrooms": -5,
      "Cocaine": -95
    },
    "properties": [
      "Lethal",
      "Euphoric",
      "ThoughtProvoking"
    ],
    "noProps": false,
    "policeRisk": 20,
    "dependence": 0.25,
    "connections": [
      "jamal_bennett"
    ],
    "unlocked": [
      "jamal_bennett"
    ],
    "avatar": "\ud83e\uddd1"
  },
  {
    "file": "VincentReeves.cs",
    "type": "customer",
    "description": "",
    "id": "vincent_reeves",
    "firstName": "Vincent",
    "lastName": "Reeves",
    "name": "Vincent Reeves",
    "isDealer": false,
    "isManager": false,
    "isSupervisor": false,
    "region": "Northtown",
    "spending": [
      400,
      700
    ],
    "orders": [
      2,
      4
    ],
    "time": "20:00",
    "standards": "Moderate",
    "standardsDisplay": "Moderate",
    "day": "Sat",
    "affinities": {
      "Marijuana": -20,
      "Methamphetamine": 50,
      "Shrooms": -30,
      "Cocaine": 50
    },
    "properties": [
      "Euphoric",
      "Glowing",
      "Energizing"
    ],
    "noProps": false,
    "policeRisk": 21,
    "dependence": 1,
    "connections": [
      "jian_ming"
    ],
    "unlocked": [
      "jian_ming"
    ],
    "avatar": "\ud83e\uddd1"
  },
  {
    "file": "WayneKerr.cs",
    "type": "customer",
    "description": "",
    "id": "wayne_kerr",
    "firstName": "Wayne",
    "lastName": "Kerr",
    "name": "Wayne Kerr",
    "isDealer": false,
    "isManager": false,
    "isSupervisor": false,
    "region": "Northtown",
    "spending": [
      400,
      700
    ],
    "orders": [
      2,
      4
    ],
    "time": "07:15",
    "standards": "High",
    "standardsDisplay": "High",
    "day": "Tue",
    "affinities": {
      "Marijuana": -82,
      "Methamphetamine": 0,
      "Shrooms": 73,
      "Cocaine": 61
    },
    "properties": [
      "Calming",
      "ThoughtProvoking",
      "Sedating"
    ],
    "noProps": false,
    "policeRisk": 28,
    "dependence": 0.25,
    "connections": [
      "peter_file",
      "ludwig_meyer"
    ],
    "unlocked": [
      "peter_file",
      "ludwig_meyer"
    ],
    "avatar": "\ud83e\uddd1"
  },
  {
    "file": "WesleyPike.cs",
    "type": "customer",
    "description": "",
    "id": "wesley_pike",
    "firstName": "Wesley",
    "lastName": "Pike",
    "name": "Wesley Pike",
    "isDealer": false,
    "isManager": false,
    "isSupervisor": false,
    "region": "Westville",
    "spending": [
      500,
      700
    ],
    "orders": [
      1,
      4
    ],
    "time": "21:00",
    "standards": "Low",
    "standardsDisplay": "Low",
    "day": "Thu",
    "affinities": {
      "Marijuana": 24,
      "Methamphetamine": 22,
      "Shrooms": -14,
      "Cocaine": 5
    },
    "properties": [
      "Energizing",
      "Foggy",
      "Smelly"
    ],
    "noProps": false,
    "policeRisk": 14,
    "dependence": 1,
    "connections": [
      "kim_delaney",
      "rhonda_vex"
    ],
    "unlocked": [
      "kim_delaney",
      "rhonda_vex"
    ],
    "avatar": "\ud83e\uddd1"
  }
];
