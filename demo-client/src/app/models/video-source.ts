import { VideoDetails } from "./video-details";

export interface VideoSource {
    title: string;
    formatDetails: string;
    showHideLabel: boolean;
    url?: string;
    videoDetails: VideoDetails;
  }