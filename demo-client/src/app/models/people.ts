export interface Face {
    imageUrl: string;
    notches: Notch[];
    name: string;
    appearancePercentage?:string;
    isSelected?:boolean;
  }
  
  export interface Notch {
    time: number;
  }