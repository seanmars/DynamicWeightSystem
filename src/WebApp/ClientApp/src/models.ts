export interface FishData {
  id: string;
  fishCode: string;
  name: string;
}

export interface FishWeightHistory {
  id: string;
  timestamp: number;
  fishCode: string;
  weight: number;
}

export interface WeightLevel {
  level: number;
  lowerBound: number;
  upperBound: number;
}

export type WeightLevelList = WeightLevel[];
