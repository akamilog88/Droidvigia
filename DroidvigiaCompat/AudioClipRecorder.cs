using System;
using System.Threading;
using System.Threading.Tasks;

using Android.Util;
using Android.Media;

namespace DroidToneDecoder
{
	public class AudioClipRecorder
	{
		private const String TAG = "AudioClipRecorder";
		private AudioRecord recorder;
		private AudioClipListener clipListener;
		/**
		* state variable to control starting and stopping recording
		*/
		private bool continueRecording;
		public  const int RECORDER_SAMPLERATE_CD = 44100;
		public  const int RECORDER_SAMPLERATE_8000 = 8000;
		private const int DEFAULT_BUFFER_INCREASE_FACTOR = 3;
		 
		private Task task;
		private bool heard;

		public AudioClipRecorder(AudioClipListener clipListener, Task task)
		{
			this.clipListener = clipListener;
			heard = false;
			this.task = task;
		}
		/**
		* records with some default parameters
		*/
		public bool startRecording()
		{
		/*return startRecording(RECORDER_SAMPLERATE_8000,
		AudioFormat.ENCODING_PCM_16BIT);
		*/
			return startRecording (RECORDER_SAMPLERATE_CD,Encoding.Pcm16bit);
		}
		/**
		* start recording: set the parameters that correspond to a buffer that
		* contains millisecondsPerAudioClip milliseconds of samples
		*/
		public bool startRecordingForTime(int millisecondsPerAudioClip,
		                                     int sampleRate, Encoding encoding)
		{
			float percentOfASecond = (float) millisecondsPerAudioClip / 1000.0f;
			int numSamplesRequired = (int) ((float) sampleRate * percentOfASecond);
			int bufferSize =
				determineCalculatedBufferSize(sampleRate, encoding,
				                              numSamplesRequired);
			return doRecording(sampleRate, encoding, bufferSize,
			                   numSamplesRequired, DEFAULT_BUFFER_INCREASE_FACTOR);
		}
		/**
		* start recording: Use a minimum audio buffer and a read buffer of the same
		* size.
		*/
		public bool startRecording(int sampleRate, Encoding encoding)
		{
		int bufferSize = determineMinimumBufferSize(sampleRate, encoding);
		return doRecording(sampleRate, encoding, bufferSize, bufferSize,
		DEFAULT_BUFFER_INCREASE_FACTOR);
		}
		private int determineMinimumBufferSize(int sampleRate, Encoding encoding)
		{
		/*int minBufferSize =
		AudioRecord.GetMinBufferSize(sampleRate,
		AudioFormat.CHANNEL_IN_MONO, encoding);
		return minBufferSize;*/
			return AudioRecord.GetMinBufferSize (sampleRate, ChannelIn.Mono, encoding);
		}

		/**
		* Calculate audio buffer size such that it holds numSamplesInBuffer and is
		* bigger than the minimum size<br>
		*
		* @param numSamplesInBuffer
		* Make the audio buffer size big enough to hold this many
		* samples
		*/
		private int determineCalculatedBufferSize(int sampleRate,
		                                          Encoding encoding, int numSamplesInBuffer)
		{
			int minBufferSize = determineMinimumBufferSize(sampleRate, encoding);
			int bufferSize;
			// each sample takes two bytes, need a bigger buffer
			if (encoding == Encoding.Pcm16bit)
			{
				bufferSize = numSamplesInBuffer * 2;
			}
			else
			{
				bufferSize = numSamplesInBuffer;
			}
			if (bufferSize < minBufferSize)
			{
				Log.Debug(TAG, "Increasing buffer to hold enough samples "
				      + minBufferSize + " was: " + bufferSize);
				bufferSize = minBufferSize;
			}
			return bufferSize;
		}
		/**
		* Records audio until stopped the {@link #task} is canceled,
		* {@link #continueRecording} is false, or {@link #clipListener} returns
		* true <br>
		* records audio to a short [readBufferSize] and passes it to
		* {@link #clipListener} <br>
		* uses an audio buffer of size bufferSize * bufferIncreaseFactor
		*
		* @param recordingBufferSize
		* minimum audio buffer size
		* @param readBufferSize
		* reads a buffer of this size
		* @param bufferIncreaseFactor
		* to increase recording buffer size beyond the minimum needed
		*/
		private bool doRecording(int sampleRate, Encoding encoding,
			int recordingBufferSize, int readBufferSize,
			int bufferIncreaseFactor)
			{
			if (recordingBufferSize < 0)
			{
				Log.Debug(TAG, "Bad encoding value, see logcat.Code="+recordingBufferSize.ToString());
			return false;
			}
			
			// give it extra space to prevent overflow
			int increasedRecordingBufferSize =
			recordingBufferSize * bufferIncreaseFactor;
			recorder =
			new AudioRecord(AudioSource.Mic, sampleRate,
			ChannelIn.Mono, encoding,
			increasedRecordingBufferSize);
			byte[] readBuffer = new byte[readBufferSize];
			continueRecording = true;
			Log.Debug(TAG, "start recording, " + "recording bufferSize: "
			      + increasedRecordingBufferSize
			      + " read buffer size: " + readBufferSize);
			//Note: possible IllegalStateException
			//if audio recording is already recording or otherwise not available
			//AudioRecord.getState() will be AudioRecord.STATE_UNINITIALIZED
			recorder.StartRecording ();
			while (continueRecording)
			{
				Thread.Sleep (5);
				int bufferResult = recorder.Read(readBuffer, 0, readBufferSize);
				//in case external code stopped this while read was happening
				if ((!continueRecording) || ((task != null) && task.IsCanceled))
				{
					break;
				}
				// check for error conditions
				if (bufferResult <0)
				{
					Log.Error(TAG, "error reading: ERROR_INVALID_OPERATION.Code="+bufferResult);
				}
				else
					// no errors, do processing
				{
					clipListener.heard(readBuffer, sampleRate);
				}
			}
			done();
			return heard;
		}

		public bool isRecording()
		{
			return continueRecording;
		}
		public void stopRecording()
		{
			continueRecording = false;
		}
		/**
		* need to call this when completely done with recording
		*/
		public void done()
		{
			Log.Debug(TAG, "shut down recorder");
			if (recorder != null)
			{
			recorder.Stop();
			recorder.Release();
			recorder = null;
			}
		}
		/*class UpdateListener:AudioRecord.IOnRecordPositionUpdateListener{
			void AudioRecord.IOnRecordPositionUpdateListener.OnPeriodicNotification(AudioRecord recorder)
			{
				// no need to read the audioData again since it was just
				// read
				heard = clipListener.heard(audioData, sampleRate);
				if (heard)
				{
					//Log.d(TAG, "heard audio");
					stopRecording();
				}
			}

			void AudioRecord.IOnRecordPositionUpdateListener.OnMarkerReached(AudioRecord recorder)
			{
				//Log.d(TAG, "marker reached");
			}
		}
		private void setOnPositionUpdate(short[] audioData,
		                                 int sampleRate, int numSamplesInBuffer)
		{
			recorder.SetPositionNotificationPeriod(numSamplesInBuffer);
			recorder.SetRecordPositionUpdateListener(new UpdateListener());
		}*/
		/**
		* @param audioData
		* will be filled when reading the audio data
		*/

		/*private void setOnPositionUpdate(short[] audioData,
		                                 int sampleRate, int numSamplesInBuffer)
		{
			OnRecordPositionUpdateListener positionUpdater =
				new OnRecordPositionUpdateListener()
			{
				@Override
				public void onPeriodicNotification(AudioRecord recorder)
				{
					// no need to read the audioData again since it was just
					// read
					heard = clipListener.heard(audioData, sampleRate);
					if (heard)
					{
						Log.d(TAG, "heard audio");
						stopRecording();
					}
				}
				@Override
				public void onMarkerReached(AudioRecord recorder)
				{
					Log.d(TAG, "marker reached");
				}
			};

			recorder.setPositionNotificationPeriod(numSamplesInBuffer);
			recorder.setRecordPositionUpdateListener(positionUpdater);
		}*/
	}
}

