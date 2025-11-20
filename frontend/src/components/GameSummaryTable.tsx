import { FinishedGameDto } from '../models/game/FinishedGameDto'

interface GameSummaryTableProps {
	finishedGameData: FinishedGameDto
	highlightedRound: number | null
	onRoundHover: (roundIndex: number | null) => void
}

const GameSummaryTable = ({
	finishedGameData,
	highlightedRound,
	onRoundHover,
}: GameSummaryTableProps) => {
	const calculateDistance = (lat1: number, lon1: number, lat2: number, lon2: number): number => {
		const R = 6371
		const dLat = ((lat2 - lat1) * Math.PI) / 180
		const dLon = ((lon2 - lon1) * Math.PI) / 180
		const a =
			Math.sin(dLat / 2) * Math.sin(dLat / 2) +
			Math.cos((lat1 * Math.PI) / 180) *
				Math.cos((lat2 * Math.PI) / 180) *
				Math.sin(dLon / 2) *
				Math.sin(dLon / 2)
		const c = 2 * Math.atan2(Math.sqrt(a), Math.sqrt(1 - a))
		return R * c
	}

	return (
		<table>
			<thead>
				<tr>
					<th>Round</th>
					<th>Place</th>
					<th>Distance</th>
					<th>Score</th>
				</tr>
			</thead>
			<tbody>
				{finishedGameData.rounds.map((round, index) => {
					const distance = calculateDistance(
						round.latitude,
						round.longitude,
						round.placeToGuess.latitude,
						round.placeToGuess.longitude
					)
					return (
						<tr
							key={index}
							className={highlightedRound === index ? 'highlighted' : ''}
							onMouseEnter={() => onRoundHover(index)}
							onMouseLeave={() => onRoundHover(null)}
						>
							<td className="round-number">{index + 1}</td>
							<td className="place-name">{round.placeToGuess.name}</td>
							<td className="distance">{distance.toFixed(2)} km</td>
							<td className="score">{round.score.toFixed(2)}</td>
						</tr>
					)
				})}
			</tbody>
		</table>
	)
}

export default GameSummaryTable
