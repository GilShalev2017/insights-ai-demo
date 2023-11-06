export class SubjectDetailsWithNameAndScores {
    name?: string;
    timeRangeAndScores?: TimeRangeAndScore[];
  }

export class TimeRangeAndScore {
    from?: string;
    to?: string;
    score?: number;
    videoName?:string;
  }

  export class TimeRangeAndScore2{
    from!: Date;
    to!: Date;
    score?: number;
    duration?:number;
  }