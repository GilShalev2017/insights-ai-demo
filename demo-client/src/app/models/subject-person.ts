import { TimeRangeAndScore, TimeRangeAndScore2 } from "./SubjectDetailsWithNameAndScores";
import { VideoDetails } from "./video-details";

export class SubjectPerson {
  time?: string;
  status?: string;
  image1?: string;
  subjectIdentified?: boolean;
  identifiedPersonName?: string;
  score?: string;
  timeRangeAndScoreArray?: TimeRangeAndScore[];
  timeRangeAndScoreArray2?: TimeRangeAndScore2[];
  videoDetails?: VideoDetails;
}