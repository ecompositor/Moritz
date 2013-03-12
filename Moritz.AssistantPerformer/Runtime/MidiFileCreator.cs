
using System;
using System.Collections.Generic;

using Multimedia.Midi;

namespace Moritz.AssistantPerformer.Runtime
{
	internal partial class MidiOutputDevice : IMidiSink
	{
		/// <summary>
		/// This class, which is embeded in MidiOutputDevice, writes MIDI files.
		/// </summary>
		private class MidiFileCreator
		{
            public MidiFileCreator(MidiOutputDevice hostDevice, SaveSequenceAsMidiFileDelegate saveSequenceAsMidiFile, string defaultFilename)
			{
				_hostMidiOutputDevice = hostDevice;
                SaveSequenceAsMidiFile = saveSequenceAsMidiFile;
                _defaultFilename = defaultFilename;

				_sequence = new Sequence(_ppqn);

				for(int i = 0 ; i < _maxTracks ; i++)
				{
					Track track = new Track();
                    track.Clear();
					_tracks.Add(track);
				}

				_clock.Ppqn = _ppqn;
				_clock.Tick += new EventHandler(IncrementAbsoluteTicks);

			}

			public void StartRecording()
			{
				for(int i = 0 ; i < _maxTracks ; i++)
				{
					if(_sequence.Contains(_tracks[i]))
					{
                        _tracks[i].Clear();
					}
				}
				_sequence.Clear();
				_absoluteTicks = 0;
				_clock.Start();
			}

			public void ProcessMessage(ChannelMessage message)
			{
				_tracks[message.MidiChannel].Insert(_absoluteTicks, message);
			}
			public void ProcessMessage(SysExMessage message)
			{
				_tracks[0].Insert(_absoluteTicks, message);
			}
			public void ProcessMessage(SysCommonMessage message)
			{
				_tracks[0].Insert(_absoluteTicks, message);
			}
			public void ProcessMessage(SysRealtimeMessage message)
			{
				_tracks[0].Insert(_absoluteTicks, message);
			}

			public void StopRecording()
			{
				for(int i = 0 ; i < _maxTracks ; i++)
				{
					if(_tracks[i].Length > 0)
					{
						FinalizeTrack(_tracks[i], i);
						_sequence.Add(_tracks[i]);
					}
				}
                if(SaveSequenceAsMidiFile != null)
                    SaveSequenceAsMidiFile(_sequence, _defaultFilename);
				_clock.Stop();
			}

            private SaveSequenceAsMidiFileDelegate SaveSequenceAsMidiFile = null;

			/// <summary>
			/// Messages are added to the end of the track
			/// </summary>
			/// <param name="track"></param>
			/// <param name="trackIndex"></param>
			private void FinalizeTrack(Track track, int trackIndex)
			{
                ChannelMessageBuilder builder = new ChannelMessageBuilder();
                builder.Command = ChannelCommand.Controller;
                builder.MidiChannel = trackIndex;

                builder.Data1 = (int)ControllerType.AllSoundOff;
                builder.Build();
                track.Insert(_absoluteTicks, builder.Result);

                builder.Data1 = (int)ControllerType.AllControllersOff;
                builder.Build();
                track.Insert(_absoluteTicks, builder.Result);
			}

			private void IncrementAbsoluteTicks(object o, EventArgs e)
			{
				_absoluteTicks++;
			}

			MidiOutputDevice _hostMidiOutputDevice;

            private string _defaultFilename = null;
			private List<Track> _tracks = new List<Track>();
			private Sequence _sequence;
			private MidiInternalClock _clock = new MidiInternalClock();
			private int _absoluteTicks = 0;

			private readonly int _maxTracks = 16;
			private readonly static int _ppqn = 240; // 5760 (= 24 * 240) would be 1 microsecond per Clock.
		}
	}
}
